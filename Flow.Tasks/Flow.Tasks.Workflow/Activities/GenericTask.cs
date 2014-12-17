using System.Reflection;
using System.Activities;
using log4net;


namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Generic Asynchronous activity
    /// </summary>
    public class GenericTask : NativeActivity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<string> TaskCode   { get; set; }
        public ActivityAction<TaskStateData> OnRun { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddDelegate(OnRun);

            var runtimeArgument = new RuntimeArgument("TaskCode", typeof(string), ArgumentDirection.In);
            metadata.Bind(TaskCode, runtimeArgument);
            metadata.AddArgument(runtimeArgument);

        }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("GenericTask -> Start");

            var workflowStatus = context.GetExtension<WorkflowStateData>();

            if (OnRun != null)
            {
                TaskStateData taskState = null;
                if (workflowStatus.Tasks.ContainsKey(TaskCode.Get(context)))
                {
                    taskState = workflowStatus.Tasks[TaskCode.Get(context)];
                }
                context.ScheduleAction(OnRun, taskState);
            }

            Log.Debug("GenericTask -> End");

        }

    }
}