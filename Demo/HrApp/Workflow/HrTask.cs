using System.Reflection;
using System.Activities;
using Flow.Tasks.Workflow.Activities;
using log4net;

namespace HrWorkflow
{
    public sealed class HrTask : Activity
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

        public HrTask()
        {
            Implementation = () => new ApproveTask
            {
                AssignedToUsers = "{r.HR}",
                CorrelationId = CorrelationId,
                Description = "A new Resume is waiting for you to be reviewed. The Resume is attached to this task.",
                DisplayName = "HrView",
                TaskCode = TaskCode,
                Title = "Review Candidate's Resume",
                UiCode = "HrView",
                ExpiresIn = "3d",
                OnInit = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onInit,
                    Handler = new OnInitHrTask
                    {
                        DisplayName = "OnInitHrTask",
                        Request = _onInit
                    }
                },
                OnComplete = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onComplete,
                    Handler = new OnCompleteHrTask
                    {
                        DisplayName = "OnCompleteHrTask",
                        Request = _onComplete
                    }
                }
            };
        }
    }

    internal sealed class OnInitHrTask : CodeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override TaskStateData Execute(CodeActivityContext context)
        {

            Log.Debug("OnInitHrTask -> Start");
            
            TaskStateData taskStatus = Request.Get(context);

            Log.Debug("OnInitHrTask -> End");
            
            return taskStatus;                
        }
    }

    internal sealed class OnCompleteHrTask : CodeActivity<TaskStateData>
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