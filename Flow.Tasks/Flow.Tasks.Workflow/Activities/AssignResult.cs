using System.Reflection;
using System.Activities;
using log4net;


namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Assign Result Activity
    /// </summary>
    public class AssignResult : NativeActivity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OutArgument<string> TaskResult { get; set; }
        public InArgument<string> TaskCode   { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("AssignResult -> Start");

            var workflowStatus = context.GetExtension<WorkflowStateData>();

            TaskResult.Set(context, workflowStatus.Tasks[TaskCode.Get(context)].Result);

            Log.Debug("AssignResult -> End");

        }

    }
}