using System;
using System.Collections.Generic;
using log4net;
using System.Activities;
using System.Activities.Statements;
using Flow.Tasks.Contract.Message;
using System.Reflection;

namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Invoke Child Workflow
    /// </summary>
    /// <remarks>
    /// Activity used to spawn a new workflow.
    /// </remarks>
    public sealed class InvokeChildWorkflow : NativeActivity
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InvokeChildWorkflow));

        /// <summary>
        /// Synchronize activity
        /// </summary>
        private Sequence _sync;
        
        /// <summary>
        /// Complete event
        /// </summary>
        private SendReceiveBase<WorkflowEventRequest, WorkflowEventResponse> _eventCompleted;

        /// <summary>
        /// Workflow Event Request
        /// </summary>
        private readonly DelegateInArgument<WorkflowEventRequest> _request = new DelegateInArgument<WorkflowEventRequest>();

        /// <summary>
        /// Workflow Code
        /// </summary>
        public InArgument<string> WorkflowCode { get; set; }

        /// <summary>
        /// Domain
        /// </summary>
        public InArgument<string> Domain { get; set; }

        /// <summary>
        /// Properties
        /// </summary>
        public InArgument<Dictionary<string, string>> Properties { get; set; }

        /// <summary>
        /// Asynchronous
        /// </summary>
        /// <remarks>
        /// If True parent workflow will wait for child workflow to complete.
        /// </remarks>
        public InArgument<bool> Async { get; set; }

        /// <summary>
        /// Workflow Id
        /// </summary>
        public OutArgument<string> WorkflowId { get; set; }

        /// <summary>
        /// Workflow Result
        /// </summary>
        public OutArgument<string> WorkflowResult { get; set; }

        /// <summary>
        /// Correlation Id
        /// </summary>
        public int CorrelationId { get; set; }
        
        /// <summary>
        /// Chils Workflow Id
        /// </summary>
        private Variable<string> NewWorkflowId = new Variable<string> { Name = "NewWorkflowId" };

        /// <summary>
        /// Create activity metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // this is used for synchronous childs. Parent will wait until child workflow
            // send a response.
            _eventCompleted = _eventCompleted ?? new SendReceiveBase<WorkflowEventRequest, WorkflowEventResponse>
            {
                ServiceContractName = "{http://flowtasks.com/}IFlowTasksOperations",
                OperationName = "WorkflowEvent",
                CanCreateInstance = false,
                GenerateCorrelationId = false,
                UseContentCorrelation = true,
                CorrelationId = CorrelationId,
                MessageNamespace = "urn:flowtasks:workflowevent",
                DisplayName = "WorkflowEvent",
                OnCompleting = new ActivityFunc<WorkflowEventRequest, WorkflowEventResponse>
                {
                    Argument = _request,
                    Handler = new CreateResponseForWorkflowEvent
                    {
                        DisplayName = "CreateResponseForWorkflowEvent",
                        Request = _request
                    }
                }
            };

            _sync = _sync ?? new Sequence
            {
                DisplayName = "WaitForWf",
                Activities = 
                {
                    _eventCompleted
                }
            };

            metadata.AddImplementationChild(_sync);             

            var workflowIdArgument = new RuntimeArgument("WorkflowId", typeof(string), ArgumentDirection.Out);
            metadata.Bind(WorkflowId, workflowIdArgument);
            metadata.AddArgument(workflowIdArgument);

            var resultArgument = new RuntimeArgument("WorkflowResult", typeof(string), ArgumentDirection.Out);
            metadata.Bind(WorkflowResult, resultArgument);
            metadata.AddArgument(resultArgument);

            var workflowCodeArgument = new RuntimeArgument("WorkflowCode", typeof(string), ArgumentDirection.In);
            metadata.Bind(WorkflowCode, workflowCodeArgument);
            metadata.AddArgument(workflowCodeArgument);

            var domainArgument = new RuntimeArgument("Domain", typeof(string), ArgumentDirection.In);
            metadata.Bind(Domain, domainArgument);
            metadata.AddArgument(domainArgument);

            var propertiesArgument = new RuntimeArgument("Properties", typeof(Dictionary<string, string>), ArgumentDirection.In);
            metadata.Bind(Properties, propertiesArgument);
            metadata.AddArgument(propertiesArgument);

            var asyncArgument = new RuntimeArgument("Async", typeof(bool), ArgumentDirection.In);
            metadata.Bind(Async, asyncArgument);
            metadata.AddArgument(asyncArgument);

            metadata.AddImplementationVariable(NewWorkflowId);

        }

        protected override void Execute(NativeActivityContext context)
        {
            Logger.Debug("InvokeChildWorkflow -> Start");

            var id = InvokeWorkflow(context.WorkflowInstanceId, WorkflowCode.Get(context), Domain.Get(context), Properties.Get(context), Async.Get(context));

            NewWorkflowId.Set(context, id);
            WorkflowId.Set(context, id);

            if (!Async.Get(context))
            {
                context.ScheduleActivity(_sync, OnChildWorkflowCompleteCallback);
            }

            Logger.Debug("InvokeChildWorkflow -> End");
        }

        /// <summary>
        /// OnChildWorkflowComplete Callback
        /// </summary>
        /// <remarks>
        /// Runs after Child workflow complete. Only for synchronous
        /// </remarks>
        /// <param name="context">NativeActivityContext</param>
        /// <param name="completedInstance">ActivityInstance</param>
        private void OnChildWorkflowCompleteCallback(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Logger.Debug("OnChildWorkflowCompleteCallback -> Start");

            GetWorkflowResultResponse res;
            using (var proxy = new Proxy.FlowTasksService())
            {
                res = proxy.GetWorkflowResult(new GetWorkflowResultRequest
                {
                    WorkflowId = WorkflowId.Get(context)
                });
            }

            WorkflowResult.Set(context, res.WorkflowResultInfo.Result);

            Logger.Debug("OnChildWorkflowCompleteCallback -> End");

        }

        /// <summary>
        /// Invoke Workflow
        /// </summary>
        /// <remarks>
        /// Start a new WorkflowCode workflow.
        /// </remarks>
        /// <param name="parentWorkflowId">Parent Workflow Id</param>
        /// <param name="workflowCode">Workflow Code</param>
        /// <param name="domain">Domain</param>
        /// <param name="properties">Properties</param>
        /// <param name="async">async</param>
        /// <returns>Workflow Id</returns>
        private string InvokeWorkflow(Guid parentWorkflowId, string workflowCode, string domain, Dictionary<string, string> properties, bool async)
        {

            var startWorkflowRequest = new StartWorkflowRequest
            {
                ParentWorkflowId = parentWorkflowId.ToString(),
                Domain = domain,
                WorkflowCode = workflowCode,
                WfRuntimeValues = CreatePropertiesFromDictionary(properties),
                CorrelationId = CorrelationId,
                WaitForChild = !async
            };

            StartWorkflowResponse startWorkflowResponse;
            using (var proxy = new Proxy.FlowTasksService())
            {
                startWorkflowResponse = proxy.StartWorkflow(startWorkflowRequest);
            }

            string workflowId = startWorkflowResponse.WorkflowId;

            return workflowId;
        }

        /// <summary>
        /// Create Properties From Dictionary
        /// </summary>
        /// <param name="properties">Properties</param>
        /// <returns>List of WfProperty</returns>
        private WfProperty[] CreatePropertiesFromDictionary(Dictionary<string, string> properties)
        {
            var wfProperties = new List<WfProperty>();
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    wfProperties.Add(new WfProperty
                    {
                        Name = p.Key,
                        Type = "S",
                        Value = p.Value
                    });
                }
            }
            return wfProperties.ToArray();
        }
    }

    /// <summary>
    /// Create Response For Workflow Event
    /// </summary>
    /// <remarks>
    /// Runs after a Complete workflow event is received. Only for synchronous.
    /// </remarks>
    internal sealed class CreateResponseForWorkflowEvent : NativeActivity<WorkflowEventResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<WorkflowEventRequest> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CreateResponseForWorkflowEvent -> Start");

            Result.Set(context, new WorkflowEventResponse
            {
                Ack = "OK"
            });
            
            Log.Debug("CreateResponseForWorkflowEvent -> End");
        }

    }
}
