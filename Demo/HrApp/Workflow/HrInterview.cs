using System.Reflection;
using System.Activities;
using Flow.Tasks.Workflow.Activities;
using log4net;

namespace HrWorkflow
{
    public sealed class HrInterview : Activity
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

        public HrInterview()
        {
            Implementation = () => new ApproveTask
            {
                AssignedToUsers = "{r.HR}",
                CorrelationId = CorrelationId,
                Description = "Please book an interview with Candidate {p.CandidateName}. He/she has been selected for the position of {p.CandidateJobType}",
                DisplayName = "HrInterview",
                TaskCode = TaskCode,
                Title = "Interview Candidate {p.CandidateName}",
                UiCode = "HrInterview",
                ExpiresIn = "3d",
                OnInit = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onInit,
                    Handler = new OnInitHrInterview
                    {
                        DisplayName = "OnInitHrTask",
                        Request = _onInit
                    }
                },
                OnComplete = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onComplete,
                    Handler = new OnCompleteHrInterview
                    {
                        DisplayName = "OnCompleteHrTask",
                        Request = _onComplete
                    }
                }
            };
        }
    }

    internal sealed class OnInitHrInterview : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            Log.Debug("OnInitHrTask -> Start");

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

            Log.Debug("OnInitHrTask -> End");

            Result.Set(context, taskStatus);
        }
    }

    internal sealed class OnCompleteHrInterview : CodeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override TaskStateData Execute(CodeActivityContext context)
        {
            Log.Debug("OnCompleteHrTask -> Start");

            TaskStateData taskStatus = Request.Get(context);
                        
            Log.Debug("OnCompleteHrTask -> End");

            return taskStatus;
        }

    }
}