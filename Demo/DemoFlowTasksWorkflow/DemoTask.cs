using System.Reflection;
using System.Activities;
using Flow.Tasks.Workflow.Activities;
using log4net;

namespace DemoFlowTasksWorkflow
{
    public sealed class DemoTask : Activity
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

        public DemoTask()
        {
            Implementation = () => new ApproveTask
            {
                AssignedToUsers = "{r.Dev}",
                CorrelationId = CorrelationId,
                Description = "This is just a demo task. It shows how simple it is to create a workflow.",
                DisplayName = "Demo task",
                TaskCode = TaskCode,
                Title = "Demo task",
                UiCode = "DemoTask",
                ExpiresIn = "10d",
                OnInit = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onInit,
                    Handler = new CreateOnClientInit
                    {
                        DisplayName = "CreateOnClientInit",
                        Request = _onInit
                    }
                },
                OnComplete = new ActivityFunc<TaskStateData, TaskStateData>
                {
                    Argument = _onComplete,
                    Handler = new CreateOnClientComplete
                    {
                        DisplayName = "CreateOnClientComplete",
                        Request = _onComplete
                    }
                }
            };
        }
    }

    internal sealed class CreateOnClientInit : CodeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override TaskStateData Execute(CodeActivityContext context)
        {

            Log.Debug("CreateOnClientInit -> Start");
            
            TaskStateData taskStatus = Request.Get(context);

            /*
             * User can do all the changes here before the activity is created.
             */
            taskStatus.AddParameter("TaskProp1", "TaskVal1");
            //taskStatus.TaskInfo.ExpiresIn = "1m";

            Log.Debug("CreateOnClientInit -> End");

            
            return taskStatus;                
        }
    }

    internal sealed class CreateOnClientComplete : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("CreateOnClientComplete -> Start");

            TaskStateData taskStatus = Request.Get(context);

            WorkflowAction.SetWorkflowResult(context, "OK");

            Log.Debug("CreateOnClientComplete -> End");

            Result.Set(context, taskStatus);
        }

    }
}