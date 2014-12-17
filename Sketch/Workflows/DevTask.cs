using Flow.Tasks.Workflow.Activities;
using log4net;
using System.Activities;
using System.Reflection;

namespace Sketch.Workflows
{

    public sealed class OnCompleteDevTask : NativeActivity<TaskStateData>
    {
        private static string SKETCHDEV = "SketchDev";
        private static string DEPLOY = "Deploy";
        private static string OK = "OK";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("OnCompleteDevTask -> Start");

            TaskStateData taskStatus = WorkflowAction.GetTaskState(context, SKETCHDEV);

            if (taskStatus != null)
            {
                if (taskStatus.Result.Equals(DEPLOY))
                {
                    WorkflowAction.SetWorkflowResult(context, OK);
                }
            }

            Result.Set(context, taskStatus);

            Log.Debug("OnCompleteDevTask -> End");            
        }

    }

    public sealed class OnCompleteFixTask : NativeActivity<TaskStateData>
    {
        private static string SKETCHFIX = "SketchFix";
        private static string REJECT = "Reject";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("OnCompleteDevTask -> Start");

            TaskStateData taskStatus;
            taskStatus = WorkflowAction.GetTaskState(context, SKETCHFIX);

            if (taskStatus != null)
            {
                if (taskStatus.Result.Equals(REJECT))
                {
                    WorkflowAction.SetWorkflowResult(context, REJECT);
                }
            }

            Result.Set(context, taskStatus);

            Log.Debug("OnCompleteDevTask -> End");
        }

    }

}
