using System.Activities;
using System.Activities.Statements;
using System;
using System.Reflection;
using log4net;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Proxy;

namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Approve Task activity
    /// </summary>
    public sealed class ApproveTask : NativeActivity
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Send and Receive for approve task
        /// </summary>
        private SendReceiveBase<ApproveTaskRequest, ApproveTaskResponse> _sendReceiveApprove;

        /// <summary>
        /// Activity's body
        /// </summary>
        private Pick _waitForApproveCompletion;

        /// <summary>
        /// Sequence approve activities
        /// </summary>
        private Sequence _approveSequence;

        /// <summary>
        /// Initialize approve activity
        /// </summary>
        private InitializeApprove _initializeApprove;

        /// <summary>
        /// Approve task request
        /// </summary>
        private readonly DelegateInArgument<ApproveTaskRequest> _request = new DelegateInArgument<ApproveTaskRequest>();

        /// <summary>
        /// Delay before expiring
        /// </summary>
        private Variable<TimeSpan> DelaySpan = new Variable<TimeSpan> { Name = "DelaySpan" };

        /// <summary>
        /// Callback for Init
        /// </summary>
        public ActivityFunc<TaskStateData, TaskStateData> OnInit { get; set; }

        /// <summary>
        /// Callback for Complete
        /// </summary>
        public ActivityFunc<TaskStateData, TaskStateData> OnComplete { get; set; }

        /// <summary>
        /// Output result
        /// </summary>
        public OutArgument<string> AssignResultTo { get; set; }

        /// <summary>
        /// Initialize parameters
        /// </summary>
        public string TaskCode { get; set; }
        public string UiCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DefaultResult { get; set; }
        public DateTime? ExpiresWhen { get; set; }
        public string ExpiresIn { get; set; }
        public string HandOverUsers { get; set; }
        public string AssignedToUsers { get; set; }
        public int CorrelationId { get; set; }

        /// <summary>
        /// Create activity metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Initialize Approve activity
            _initializeApprove = CreateInitializeApprove();

            // Handle web service call. It will wait till task approved/rejected.
            _sendReceiveApprove = CreateSendReceiveApprove();

            // Use a pick activity to handle expiring time
            _waitForApproveCompletion = CreateWaitForApprove();

            // Approve activities
            _approveSequence = CreateApproveBody();

            // Set up the activity metadata
            SetupMetadata(metadata);
        }

        /// <summary>
        /// Activity entry point
        /// </summary>
        /// <param name="context">Activity context</param>
        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("ApproveTask -> Start");

            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus == null)
            {
                workflowStatus = context.GetExtension<WorkflowStateData>();

                if (workflowStatus == null) throw new Exception("Cannot find WorkflowStateData");

                context.Properties.Add(WorkflowStateData.Name, workflowStatus);
            }

            // persist task data 
            PersistTaskData(workflowStatus, TaskCode);

            // Start the process
            if (OnInit != null)
            {
                context.ScheduleFunc(OnInit, workflowStatus.Tasks[TaskCode], OnInitCallback);
            }
            else
            {
                // Set the expiry here before starting the approve
                SetExpiry(context, ExpiresWhen, ExpiresIn);

                context.ScheduleActivity(_approveSequence, OnCompleteCallback);
            }

            Log.Debug("ApproveTask -> End");

        }

        /// <summary>
        /// Persist state data so if workflow suspend/resume can continue
        /// </summary>
        /// <param name="workflowStatus">WorkflowStatus</param>
        /// <param name="key">TaskCode key</param>
        private void PersistTaskData(WorkflowStateData workflowStatus, string key)
        {
            if (workflowStatus.Tasks.ContainsKey(key))
            {
                workflowStatus.Tasks.Remove(key);
            }

            workflowStatus.AddTask(key,
                                    new TaskStateData
                                    {
                                        TaskInfo = new TaskInfo
                                        {
                                            AssignedToUsers = AssignedToUsers,
                                            DefaultResult = DefaultResult,
                                            Description = Description,
                                            ExpiresWhen = ExpiresWhen,
                                            ExpiresIn = ExpiresIn,
                                            HandOverUsers = HandOverUsers,
                                            TaskCode = key,
                                            TaskCorrelationId = CorrelationId,
                                            Title = Title,
                                            UiCode = UiCode
                                        }
                                    });
        }

        /// <summary>
        /// Executes after "client" OnInit callback
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="completedInstance">ActivityInstance</param>
        /// <param name="result">TaskStateData</param>
        private void OnInitCallback(NativeActivityContext context, ActivityInstance completedInstance, TaskStateData result)
        {

            Log.Debug("OnInitCallback -> Start");

            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus == null) throw new Exception("Cannot find WorkflowStateData");

            var task = workflowStatus.Tasks[TaskCode];

            SetExpiry(context, task.TaskInfo.ExpiresWhen, task.TaskInfo.ExpiresIn);

            context.ScheduleActivity(_approveSequence, OnCompleteCallback);

            Log.Debug("OnInitCallback -> End");

        }

        /// <summary>
        /// Calculate expiry date time
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="when">DateTime</param>
        /// <param name="at">number of days/minutes</param>
        private void SetExpiry(NativeActivityContext context, DateTime? when, string at)
        {
            GetExpiryTimeSpanResponse resp;
            using (var proxy = new FlowTasksService())
            {
                resp = proxy.GetExpiryTimeSpan(new GetExpiryTimeSpanRequest
                {
                    ExpiresWhen = when,
                    ExpiresIn = at
                });
            }

            // Set the expiry delay. User can have change it.
            DelaySpan.Set(context, resp.Expires);
        }

        /// <summary>
        /// Executes after task approved/rejected
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="completedInstance">ActivityInstance</param>
        private void OnCompleteCallback(NativeActivityContext context, ActivityInstance completedInstance)
        {
            Log.Debug("OnCompleteCallback -> Start");

            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus == null) throw new Exception("Cannot find WorkflowStateData");

            if (OnComplete != null)
            {
                context.ScheduleFunc(OnComplete, workflowStatus.Tasks[TaskCode], OnClientCompleteCallBack);
            }

            AssignResultTo.Set(context, workflowStatus.Tasks[TaskCode].Result);

            Log.Debug("OnCompleteCallback -> End");

        }

        /// <summary>
        /// Executes after "client" OnComplete callback 
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="completedInstance">ActivityInstance</param>
        /// <param name="result">TaskStateData</param>
        private void OnClientCompleteCallBack(NativeActivityContext context, ActivityInstance completedInstance, TaskStateData result)
        {
            Log.Debug("OnClientCompleteCallBack -> Start");

            var workflowStatusEx = context.GetExtension<WorkflowStateData>();

            var workflowStatusProp = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatusProp != null)
            {
                workflowStatusEx.Parameters = workflowStatusProp.Parameters;
                workflowStatusEx.Tasks = workflowStatusProp.Tasks;
                
                workflowStatusEx.Result = workflowStatusProp.Result;
            }

            Log.Debug("OnClientCompleteCallBack -> End");

        }

        /// <summary>
        /// Create Initialize Approve
        /// </summary>
        /// <returns>InitializeApprove</returns>
        private InitializeApprove CreateInitializeApprove()
        {
            return _initializeApprove ?? new InitializeApprove { TaskCode = TaskCode };
        }

        /// <summary>
        /// Create Send Receive Approve
        /// </summary>
        /// <returns>SendReceiveBase</returns>
        private SendReceiveBase<ApproveTaskRequest, ApproveTaskResponse> CreateSendReceiveApprove()
        {
            return _sendReceiveApprove ?? new SendReceiveBase<ApproveTaskRequest, ApproveTaskResponse>
            {
                ServiceContractName = "{http://flowtasks.com/}IFlowTasksOperations",
                OperationName = "ApproveTask",
                CanCreateInstance = false,
                GenerateCorrelationId = false,
                UseContentCorrelation = true,
                CorrelationId = CorrelationId,
                MessageNamespace = "urn:flowtasks:approvetask",
                DisplayName = "ApproveTask",
                OnCompleting = new ActivityFunc<ApproveTaskRequest, ApproveTaskResponse>
                {
                    Argument = _request,
                    Handler = new CreateResponseForApproveTask
                    {
                        DisplayName = "CreateResponseForApproveTask",
                        Request = _request
                    }
                }
            };
        }

        /// <summary>
        /// Create Approve Completion
        /// </summary>
        /// <returns>Pick</returns>
        private Pick CreateWaitForApprove()
        {
            return _waitForApproveCompletion ?? new Pick
            {
                DisplayName = "MainApproveBody",
                Branches =
                {
                    new PickBranch
                    {
                        Trigger = _sendReceiveApprove,
                        Action = new CompleteActionApproveTask
                        {
                            DisplayName = "CompleteActionApproveTask",
                            TaskCode = TaskCode
                        }          
                    },
                    new PickBranch
                    {
                        Trigger = new Delay
                        {
                            Duration = DelaySpan
                        },
                        Action = new CompleteExpiredApproveTask
                        {
                            DisplayName = "CompleteExpiredApproveTask",
                            DefaultResult = "Expired", // TODO: move this...
                            TaskCode = TaskCode
                        }                               
                    }
                }
            };
        }

        /// <summary>
        /// Create Approve Body
        /// </summary>
        /// <returns>Sequence</returns>
        private Sequence CreateApproveBody()
        {
            return _approveSequence ?? new Sequence
            {
                DisplayName = "ApproveSequence",
                Activities =
                {
                    _initializeApprove,
                    _waitForApproveCompletion,
                }
            };
        }

        /// <summary>
        /// Add activities, variables and argument to metadata
        /// </summary>
        /// <param name="metadata">Metadata</param>
        private void SetupMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddDelegate(OnInit);
            metadata.AddDelegate(OnComplete);
            metadata.AddImplementationChild(_approveSequence);
            metadata.AddDefaultExtensionProvider(() => new WorkflowStateData());
            metadata.AddImplementationVariable(DelaySpan);
            var argAssignTo = new RuntimeArgument("AssignResultTo", typeof(string), ArgumentDirection.Out);
            metadata.Bind(AssignResultTo, argAssignTo);
            metadata.AddArgument(argAssignTo);
        }


    }


    /// <summary>
    /// created as soon as the request to approve/reject arrives
    /// </summary>
    internal sealed class CreateResponseForApproveTask : NativeActivity<ApproveTaskResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<ApproveTaskRequest> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CreateResponseForApproveTask -> Start");

            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus == null) throw new Exception("Cannot find WorkflowStateData");

            var task = workflowStatus.Tasks[Request.Get(context).TaskCode];

            task.Result = Request.Get(context).Result;
            task.AssignedUser = Request.Get(context).UserName;

            var parameters = Request.Get(context).Parameters;
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    task.AddParameter(item.Name, item.Value);
                }
            }

            using (var proxy = new FlowTasksService())
            {
                proxy.CompleteTask(new CompleteTaskRequest
                {
                    TaskId = Request.Get(context).TaskId,
                    Result = Request.Get(context).Result,
                    User = Request.Get(context).UserName
                });
            }

            Log.Debug("CreateResponseForApproveTask -> End");

            Result.Set(context, new ApproveTaskResponse
            {
                WorkflowId = Request.Get(context).WorkflowId,
                TaskId = Request.Get(context).TaskId
            });
        }

    }

    /// <summary>
    /// created as soon as task expired
    /// </summary>
    internal sealed class CompleteExpiredApproveTask : NativeActivity<string>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<string> TaskCode { get; set; }
        public InArgument<string> DefaultResult { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CompleteExpiredApproveTask -> Start");

            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus == null) throw new Exception("Cannot find WorkflowStateData");

            using (var proxy = new FlowTasksService())
            {
                proxy.CompleteTask(new CompleteTaskRequest
                {
                    TaskId = workflowStatus.Tasks[TaskCode.Get(context)].TaskInfo.TaskOid.ToString(),
                    Result = DefaultResult.Get(context),
                    User = ""
                });
            }

            Log.Debug("CompleteExpiredApproveTask -> End");

            Result.Set(context, DefaultResult.Get(context));
        }

    }

    /// <summary>
    /// created as soon as task completed
    /// </summary>
    internal sealed class CompleteActionApproveTask : NativeActivity<string>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<string> TaskCode { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CompleteActionApproveTask -> Start");


            Log.Debug("CompleteActionApproveTask -> End");
        }

    }
}
