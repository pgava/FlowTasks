using System.Reflection;
using System.Activities;
using Flow.Tasks.Workflow.Activities;
using log4net;

namespace ServiceWorkflows
{
    public sealed class MyApproveTask : Activity
    {
        private readonly DelegateInArgument<TaskStateData> _onInit = new DelegateInArgument<TaskStateData>();
        private readonly DelegateInArgument<TaskStateData> _onComplete = new DelegateInArgument<TaskStateData>();
        public OutArgument<string> AssignResultTo { get; set; }

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

        public MyApproveTask()
        {
            Implementation = () => new ApproveTask
            {
                AssignedToUsers = "{r.Dev}",
                CorrelationId = CorrelationId,
                DefaultResult = "OK",
                Description = "This is desc for my approve task",
                DisplayName = "Approve my  task",
                TaskCode = TaskCode,
                Title = "This is the title for my approve",
                UiCode = "ApproveTask",
                ExpiresIn = "30m",
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

            taskStatus.AddParameter("TaskProp1", "TaskVal1");

            WorkflowAction.SetWorkflowResult(context, "OK");

            // Example how to use notification
            //WorkflowAction.SendNotification(context.WorkflowInstanceId, taskStatus.TaskInfo.TaskOid,
            //    "Notification", "This is the body of the message", "{r.Dev}");

            Result.Set(context, taskStatus);

            Log.Debug("CreateOnClientComplete -> End");            
        }

    }
}