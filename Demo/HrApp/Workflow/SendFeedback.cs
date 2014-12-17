using System.Reflection;
using System.Activities;
using Flow.Tasks.Workflow.Activities;
using log4net;

namespace HrWorkflow
{
    public class SendFeedback : NativeActivity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("AssignResult -> Start");

            // send emal to user

            Log.Debug("AssignResult -> End");

        }

    }
}
