using System.Reflection;
using System.Activities;
using log4net;


namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Assign Parameter Activity
    /// </summary>
    public class AssignParameter : NativeActivity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OutArgument<string> TaskParameter { get; set; }
        public InArgument<string> TaskCode   { get; set; }
        public InArgument<string> TaskParameterName { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("AssignResult -> Start");

            var workflowStatus = context.GetExtension<WorkflowStateData>();

            var taskCode = TaskCode.Get(context);

            if (!string.IsNullOrWhiteSpace(taskCode) && workflowStatus.Tasks.ContainsKey(taskCode))
            {
                TaskParameter.Set(context, workflowStatus.Tasks[taskCode].Parameters[TaskParameterName.Get(context)]);
            }
            else
            {
                TaskParameter.Set(context, workflowStatus.Parameters[TaskParameterName.Get(context)]);
            }
            

            Log.Debug("AssignResult -> End");

        }

    }
}