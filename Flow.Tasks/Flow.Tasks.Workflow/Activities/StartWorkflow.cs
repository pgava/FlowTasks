using System.Activities;
using System.Activities.Statements;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using log4net;
using System.Text;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Workflow.Activities
{

    /// <summary>
    /// Start Workflow
    /// </summary>
    /// <remarks>
    /// There is a problem/limitation with variables. All the activities inside StartWorkflow
    /// cannot access variables defined in the outer scope.
    /// </remarks>
    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation")]
    public sealed class StartWorkflow : NativeActivity
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartWorkflow));

        /// <summary>
        /// Start Workflow Request
        /// </summary>
        private readonly DelegateInArgument<StartWorkflowRequest> _request = new DelegateInArgument<StartWorkflowRequest>();

        /// <summary>
        /// Exception
        /// </summary>
        private readonly DelegateInArgument<Exception> _ex = new DelegateInArgument<Exception>();

        /// <summary>
        /// Store the request
        /// </summary>
        private readonly Variable<StartWorkflowRequest> _currentRequest = new Variable<StartWorkflowRequest> { Name = "CurrentRequest" };

        /// <summary>
        /// List of activities added by user
        /// </summary>
        private readonly Sequence _userWorkflowSequence = new Sequence();

        /// <summary>
        /// Start Workflow Sequence
        /// </summary>
        private Sequence _startWorkflowSequence;

        /// <summary>
        /// The pick activity that handles user terminate request.
        /// </summary>
        private Pick _startWorkflowBody;

        /// <summary>
        /// Terminate request activity
        /// </summary>
        private SendReceiveBase<TerminateWorkflowRequest, TerminateWorkflowResponse> _terminate;

        /// <summary>
        /// Terminate request message
        /// </summary>
        private readonly DelegateInArgument<TerminateWorkflowRequest> _requestTerminate = new DelegateInArgument<TerminateWorkflowRequest>();

        /// <summary>
        /// Start Workflow Catch
        /// </summary>
        private TryCatch _startWorkflowCatch;

        /// <summary>
        /// Workflow Id 
        /// </summary>
        public OutArgument<string> WorkflowId { get; set; }

        /// <summary>
        /// Workflow Result
        /// </summary>
        public OutArgument<string> WorkflowResult { get; set; }

        /// <summary>
        /// User Activities
        /// </summary>
        [Browsable(false)]
        public Collection<Activity> Activities
        {
            get
            {
                return _userWorkflowSequence.Activities;
            }
        }

        /// <summary>
        /// User Variables
        /// </summary>
        [Browsable(false)]
        public Collection<Variable> Variables
        {
            get
            {
                return _userWorkflowSequence.Variables;
            }
        }

        /// <summary>
        /// Store correlation Id
        /// </summary>
        private int _correlationId = 1;

        /// <summary>
        /// CorrelationId
        /// </summary>
        public int CorrelationId
        {
            get
            {
                return _correlationId;
            }
            set
            {
                _correlationId = value;
            }
        }

        /// <summary>
        /// Create activity metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // wait to start request and then run client activities
            _startWorkflowSequence = CreateStartWorkflowSequence();

            // wait for any terminate request
            _terminate = CreateTerminateSendReceive();

            // Use a Pick activity to handle user terminate requests
            _startWorkflowBody = CreateStartWorkflowBody();

            // wrap everything in a catch activity
            _startWorkflowCatch = CreateStartWorkflowCatch();

            // Set up the activity metadata
            SetupMetadata(metadata);
        }

        /// <summary>
        /// Add activities, variables and argument to metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        private void SetupMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddImplementationChild(_startWorkflowCatch);
            var workflowIdArgument = new RuntimeArgument("WorkflowId", typeof(string), ArgumentDirection.Out);
            metadata.Bind(WorkflowId, workflowIdArgument);
            metadata.AddArgument(workflowIdArgument);
            var resultArgument = new RuntimeArgument("WorkflowResult", typeof(string), ArgumentDirection.Out);
            metadata.Bind(WorkflowResult, resultArgument);
            metadata.AddArgument(resultArgument);
            metadata.AddImplementationVariable(_currentRequest);
        }

        /// <summary>
        /// Create StartWorkflow Catch
        /// </summary>
        /// <returns>TryCatch</returns>
        private TryCatch CreateStartWorkflowCatch()
        {
            _startWorkflowCatch = _startWorkflowCatch ?? new TryCatch
            {
                DisplayName = "Workflow Main Activity",
                Try = _startWorkflowBody,
                Catches = 
                {
                    new Catch<Exception>
                {
                        Action = new ActivityAction<Exception>
                    {
                            Argument = _ex,
                            DisplayName = "ActivityAction - Exception",
                            Handler = new CatchActivity
                        {                            
                                DisplayName = "CatchActivity",
                                Request = _ex
                            }
                        }
                    }
                },
                Finally = new FinalActivity
                            {
                                DisplayName = "FinalActivity",
                                CurrentRequest = _currentRequest
                            }
            };

            return _startWorkflowCatch;
        }

        /// <summary>
        /// Create StartWorkflow Body
        /// </summary>
        /// <returns>Pick</returns>
        private Pick CreateStartWorkflowBody()
        {
            _startWorkflowBody = _startWorkflowBody ?? new Pick
            {
                DisplayName = "StartWorkflowBody",
                Branches =
                {
                    new PickBranch
                    {
                        Trigger = _startWorkflowSequence                               
                    },
                    new PickBranch
                    {
                        Trigger = _terminate                           
                    }
                }
            };

            return _startWorkflowBody;
        }

        /// <summary>
        /// Create Terminate SendReceive
        /// </summary>
        /// <returns>SendReceiveBase</returns>
        private SendReceiveBase<TerminateWorkflowRequest, TerminateWorkflowResponse> CreateTerminateSendReceive()
        {
            _terminate = _terminate ?? new SendReceiveBase<TerminateWorkflowRequest, TerminateWorkflowResponse>
            {
                ServiceContractName = "{http://flowtasks.com/}IFlowTasksOperations",
                OperationName = "TerminateWorkflow",
                CanCreateInstance = false,
                GenerateCorrelationId = false,
                UseContentCorrelation = true,
                CorrelationId = CorrelationId,
                MessageNamespace = "urn:flowtasks:terminateworkflow",
                DisplayName = "TerminateWorkflow",
                OnCompleting = new ActivityFunc<TerminateWorkflowRequest, TerminateWorkflowResponse>
                {
                    Argument = _requestTerminate,
                    Handler = new CreateResponseForTerminateWorkflow
                    {
                        DisplayName = "CreateResponseForTerminateWorkflow",
                        Request = _requestTerminate
                    }
                }
            };

            return _terminate;
        }

        /// <summary>
        /// Create StartWorkflow Sequence
        /// </summary>
        /// <returns>Sequence</returns>
        private Sequence CreateStartWorkflowSequence()
        {
            _startWorkflowSequence = _startWorkflowSequence ?? new Sequence
            {
                DisplayName = "StartWorkflowActivity",
                Activities =
                {
                    new SendReceiveBase<StartWorkflowRequest, StartWorkflowResponse>
                    {
                        ServiceContractName = "{http://flowtasks.com/}IFlowTasksOperations",
                        OperationName = "StartWorkflow",
                        CanCreateInstance = true,
                        GenerateCorrelationId = true,
                        UseContentCorrelation = true,
                        CorrelationId = CorrelationId,
                        MessageNamespace = "urn:flowtasks:startworkflow",
                        DisplayName = "StartWorkflow",
                        OnCompleting = new ActivityFunc<StartWorkflowRequest, StartWorkflowResponse>
                        {
                            Argument = _request,
                            Handler = new CreateResponseForStartWorkflow
                            {
                                DisplayName = "CreateResponse",
                                Request = _request,
                                CurrentRequest = _currentRequest
                        }
                    }
                },
                    _userWorkflowSequence,
                }
            };

            return _startWorkflowSequence;
        }

        /// <summary>
        /// Activity entry point
        /// </summary>
        /// <param name="context">Activity context</param>
        protected override void Execute(NativeActivityContext context)
        {
            Logger.Debug("StartWorkflow -> Start");

            WorkflowId.Set(context, context.WorkflowInstanceId.ToString());

            context.ScheduleActivity(_startWorkflowCatch, OnWorkflowCompleteCallback);

            Logger.Debug("StartWorkflow -> End");
        }

        /// <summary>
        /// OnWorkflowComplete Callback
        /// </summary>
        /// <remarks>
        /// Runs after everything else complete.
        /// </remarks>
        /// <param name="context">NativeActivityContext</param>
        /// <param name="completedInstance">ActivityInstance</param>
        private void OnWorkflowCompleteCallback(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Logger.Debug("OnWorkflowCompleteCallback -> Start");

            var workflowStatus = context.GetExtension<WorkflowStateData>();

            WorkflowResult.Set(context, workflowStatus.Result);

            Logger.Debug("OnWorkflowCompleteCallback -> End");
        }

    }

    /// <summary>
    /// Handle exception
    /// </summary>
    internal sealed class CatchActivity : NativeActivity<Exception>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public InArgument<Exception> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CatchActivity -> Start");

            var er = new StringBuilder();
            er.AppendLine(string.Format("Message: {0}", Request.Get(context).Message));
            er.AppendLine(string.Format("InnerException: {0}", Request.Get(context).InnerException));
            er.AppendLine(string.Format("StackTrace: {0}", Request.Get(context).StackTrace));

            var workflowStatus = context.GetExtension<WorkflowStateData>();
            workflowStatus.Result = WorkflowStatusType.Aborted.ToString();

            using (var proxy = new Proxy.FlowTasksService())
            {
                proxy.CompleteWorkflow(new CompleteWorkflowRequest
                {
                    WorkflowId = context.WorkflowInstanceId.ToString(),
                    WorkflowStatusType = WorkflowStatusType.Aborted,
                    Result = workflowStatus.Result,
                    Message = er.ToString()
                });
            }

            Log.Debug(er);

            Log.Debug("CatchActivity -> End");
        }
    }

    /// <summary>
    /// Final activity
    /// </summary>
    internal sealed class FinalActivity : NativeActivity<Exception>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<StartWorkflowRequest> CurrentRequest { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("FinalActivity -> Start");

            // TODO. update any workflow Out properties
            var workflowStatus = context.GetExtension<WorkflowStateData>();

            using (var proxy = new Proxy.FlowTasksService())
            {
                proxy.CompleteWorkflow(new CompleteWorkflowRequest
                {
                    WorkflowId = context.WorkflowInstanceId.ToString(),
                    WorkflowStatusType = WorkflowStatusType.Completed,
                    Result = workflowStatus.Result
                });

                if (!string.IsNullOrWhiteSpace(CurrentRequest.Get(context).ParentWorkflowId) &&
                    CurrentRequest.Get(context).WaitForChild)
                {
                    proxy.WorkflowEvent(new WorkflowEventRequest
                    {
                        WorkflowId = CurrentRequest.Get(context).ParentWorkflowId,
                        CurrentId = context.WorkflowInstanceId.ToString(),
                        CorrelationId = CurrentRequest.Get(context).CorrelationId,
                        Result = workflowStatus.Result,
                        Event = "Completed"
                    });
                }
            }


            Log.Debug("FinalActivity -> End");

        }
    }

    /// <summary>
    /// Create Response For StartWorkflow
    /// </summary>
    /// <remarks>
    /// Runs soon after StartworkflowRequest arrives.
    /// </remarks>
    internal sealed class CreateResponseForStartWorkflow : NativeActivity<StartWorkflowResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<StartWorkflowRequest> Request { get; set; }
        public OutArgument<StartWorkflowRequest> CurrentRequest { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddDefaultExtensionProvider(() => new WorkflowStateData());
            base.CacheMetadata(metadata);
        }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CreateResponseForStartWorkflow -> Start");

            CurrentRequest.Set(context, Request.Get(context));

            var oid = CreateWorkflowFromRequest(context);

            var workflowStatus = context.GetExtension<WorkflowStateData>();

            foreach (var p in GetParametersFromRequest(Request.Get(context)))
            {
                workflowStatus.AddParameter(p.Name, p.Value);
            }

            var res = new StartWorkflowResponse
            {
                WorkflowId = oid.ToString()
            };

            Result.Set(context, res);

            Log.Debug("CreateResponseForStartWorkflow -> End");

        }

        /// <summary>
        /// Create Workflow From Request
        /// </summary>
        /// <param name="context">NativeActivityContext</param>
        /// <returns>Workflow Id</returns>
        private Guid CreateWorkflowFromRequest(NativeActivityContext context)
        {
            var parentId = Request.Get(context).ParentWorkflowId;
            var parentOid = string.IsNullOrWhiteSpace(parentId) ? Guid.Empty : Guid.Parse(parentId);

            using (var proxy = new Proxy.FlowTasksService())
            {
                proxy.CreateWorkflow(new CreateWorkflowRequest
                {
                    WorkflowId = context.WorkflowInstanceId.ToString(),
                    ParentWorkflowId = parentOid.ToString(),
                    WorkflowCode = Request.Get(context).WorkflowCode,
                    Domain = Request.Get(context).Domain,
                    PropertyInfos = new PropertyInfos(GetParametersFromRequest(Request.Get(context)))
                });
            }

            return context.WorkflowInstanceId;
        }

        /// <summary>
        /// Get Parameters From Request
        /// </summary>
        /// <param name="request">StartWorkflowRequest</param>
        /// <returns>List of PropertyInfo</returns>
        private IEnumerable<Contract.Message.PropertyInfo> GetParametersFromRequest(StartWorkflowRequest request)
        {
            var res = new List<Contract.Message.PropertyInfo>();
            if (request.WfRuntimeValues == null) return res;

            foreach (var d in request.WfRuntimeValues)
            {
                res.Add(new Contract.Message.PropertyInfo { Name = d.Name, Value = d.Value, Type = d.Type });
            }

            return res;
        }
    }

    /// <summary>
    /// Create Response For TerminateWorkflow
    /// </summary>
    /// <remarks>
    /// Runs soon after a TerminateWorkflowRequest arrives.
    /// </remarks>
    internal sealed class CreateResponseForTerminateWorkflow : NativeActivity<TerminateWorkflowResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TerminateWorkflowRequest> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CreateResponseForTerminateWorkflow -> Start");

            var workflowStatus = context.GetExtension<WorkflowStateData>();
            workflowStatus.Result = WorkflowStatusType.Terminated.ToString();

            using (var proxy = new Proxy.FlowTasksService())
            {
                proxy.CompleteWorkflow(new CompleteWorkflowRequest
                {
                    WorkflowId = context.WorkflowInstanceId.ToString(),
                    WorkflowStatusType = WorkflowStatusType.Terminated,
                    Result = workflowStatus.Result,
                    Message = Request.Get(context).Message
                });
            }

            Log.Debug("CreateResponseForTerminateWorkflow -> End");

            Result.Set(context, new TerminateWorkflowResponse
            {
                Ack = Library.Properties.Resources.SUCCESS_RESULT
            });

        }

    }
}