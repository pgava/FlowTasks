using System.Reflection;
using System.Activities;
using Flow.Tasks.Workflow.Activities;
using log4net;

namespace HrWorkflow
{
    public sealed class MgrTask : Activity
    {
        private readonly DelegateInArgument<TaskStateData> _onInit = new DelegateInArgument<TaskStateData>();
        private readonly DelegateInArgument<TaskStateData> _onComplete = new DelegateInArgument<TaskStateData>();

        private int _correlationId;
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

        private string _taskCode;
        public string TaskCode
        {
            get
            {
                return _taskCode;
            }
            set
            {
                _taskCode = value;
            }
        }

        public MgrTask()
        {
            Implementation = () => new ApproveTask
            {
                AssignedToUsers = "{r.MgrHr}",
                CorrelationId = CorrelationId,
                Description = "Please review this candidate. Is he/she suitable for the position of {p.CandidateJobType}?",
                DisplayName = "MgrView",
                TaskCode = TaskCode,
                Title = "Review Candidate {p.CandidateName}",
                UiCode = "MgrView",
                ExpiresIn = "15d",
                OnInit = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onInit,
                    Handler = new OnInitMgrTask
                    {
                        DisplayName = "OnInitMgrTask",
                        Request = _onInit
                    }
                },
                OnComplete = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onComplete,
                    Handler = new OnCompleteMgrTask
                    {
                        DisplayName = "OnCompleteMgrTask",
                        Request = _onComplete
                    }
                }
            };
        }
    }

    internal sealed class OnInitMgrTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            Log.Debug("OnInitMgrTask -> Start");
            
            TaskStateData taskStatus = Request.Get(context);
            TaskStateData hrTask = WorkflowAction.GetTaskState(context, "HrTask");

            // Get the properties from previous task (HrTask) and
            // add them to the current task.
            if (hrTask.Parameters.ContainsKey("CandidateName"))
            {
                taskStatus.Parameters.Add("CandidateName",
                    hrTask.Parameters["CandidateName"]);
            }

            if (hrTask.Parameters.ContainsKey("CandidateJobType"))
            {
                taskStatus.Parameters.Add("CandidateJobType",
                    hrTask.Parameters["CandidateJobType"]);
            }

            Log.Debug("OnInitMgrTask -> End");

            Result.Set(context, taskStatus);
        }
    }

    internal sealed class OnCompleteMgrTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("OnCompleteMgrTask -> Start");

            TaskStateData taskStatus = Request.Get(context);

            WorkflowAction.SetWorkflowResult(context, "OK");

            Log.Debug("OnCompleteMgrTask -> End");

            Result.Set(context, taskStatus);
        }

    }
}