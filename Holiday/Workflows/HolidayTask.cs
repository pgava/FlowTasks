using System.Linq;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Proxy;
using Flow.Tasks.Workflow.Activities;
using Flow.Users.Contract.Message;
using Flow.Users.Proxy;
using log4net;
using System.Activities;
using System.Reflection;

namespace Holiday.Workflows
{
    public sealed class OnRunGenericTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            Log.Debug("OnRunGenericTask -> Start");

            var req = Request.Get(context);

            var workflowStateData = context.GetExtension<WorkflowStateData>();

            using (var proxy = new FlowUsersService())
            {
                var resp = proxy.GetRolesForUser(new GetRolesForUserRequest { IsPrimaryRole = true, User = workflowStateData.Parameters[HolidayWfUtility.UserParameter] });
                var role = resp.Roles.First();

                workflowStateData.AddParameter("UserRole", string.Format("{{r.Mgr{0}}}", role));
                workflowStateData.AddParameter("IsUserMgr", role.StartsWith("Mgr") ? "Y" : "N");
            }

            Result.Set(context, req);

            Log.Debug("OnRunGenericTask -> End");

        }
    }

    public sealed class OnInitMgrTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            Log.Debug("OnInitMgrTask -> Start");

            TaskStateData taskStatus = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidaymgr);

            using (var proxy = new FlowUsersService())
            {
                var resp = proxy.GetRolesForUser(new GetRolesForUserRequest { IsPrimaryRole = true, User = taskStatus.Parameters[HolidayWfUtility.UserParameter] });
                var role = resp.Roles.First();

                Log.Debug("OnInitMgrTask -> Assign task to " + string.Format("{{r.Mgr{0}}}", role));

                taskStatus.TaskInfo.AssignedToUsers = string.Format("{{r.Mgr{0}}}", role);
            }

            // Test
            //taskStatus.TaskInfo.AssignedToUsers = "{r.MgrDev}";

            Result.Set(context, taskStatus);

            Log.Debug("OnInitMgrTask -> End");

        }
    }


    public sealed class OnInitHrTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            Log.Debug("OnInitHrTask -> Start");

            TaskStateData taskStatus = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidayhr);

            /*
             * User can do all the changes here before the activity is created.
             */
            //taskStatus.AddParameter("TaskProp1", "TaskVal1");
            //taskStatus.TaskInfo.ExpiresIn = "1m";

            Log.Debug("OnInitHrTask -> End");


            Result.Set(context, taskStatus);
        }
    }

    public sealed class OnInitUserTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

            Log.Debug("OnInitUserTask -> Start");

            TaskStateData taskStatus = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidayuser);


            TaskStateData prevTask = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidaymgr);
            if (prevTask != null && prevTask.Parameters.ContainsKey(HolidayWfUtility.ResultParameter))
            {
                var prevRes = prevTask.Parameters[HolidayWfUtility.ResultParameter];

                Log.Debug("OnInitUserTask -> Holiday result: " + prevRes);

                taskStatus.AddParameter(HolidayWfUtility.ResultParameter, prevRes);
            }
            else
            {
                prevTask = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidayhr);
                if (prevTask != null && prevTask.Parameters.ContainsKey(HolidayWfUtility.ResultParameter))
                {
                    var prevRes = prevTask.Parameters[HolidayWfUtility.ResultParameter];

                    Log.Debug("OnInitUserTask -> Holiday result: " + prevRes);

                    taskStatus.AddParameter(HolidayWfUtility.ResultParameter,
                                            prevTask.Parameters[HolidayWfUtility.ResultParameter]);
                }
            }

            var user = taskStatus.Parameters[HolidayWfUtility.UserParameter];

            Log.Debug("OnInitUserTask -> Assign task to user: " + user);

            taskStatus.TaskInfo.AssignedToUsers = string.Format("{{u.{0}}}", user);

            Log.Debug("OnInitUserTask -> End");

            Result.Set(context, taskStatus);
        }
    }

    public sealed class OnCompleteHrTask : NativeActivity<TaskStateData>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("OnCompleteHrTask -> Start");

            var taskStatus = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidayhr);

            if (taskStatus != null)
            {

                var res = HolidayStatus.A;
                if (taskStatus.Result.Equals(HolidayWfUtility.RejectResult))
                {
                    res = HolidayStatus.R;
                }

                HolidayWfUtility.SetHoliday(taskStatus, res);

                taskStatus.AddParameter(HolidayWfUtility.ResultParameter, res == HolidayStatus.R ? HolidayWfUtility.RejectMessage : HolidayWfUtility.ApproveMessage);
            }

            Result.Set(context, taskStatus);

            Log.Debug("OnCompleteHrTask -> End");            
        }       
    }

    public sealed class OnCompleteMgrTask : NativeActivity<TaskStateData>
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("OnCompleteMgrTask -> Start");

            var taskStatus = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidaymgr);

            if (taskStatus != null)
            {
                if (taskStatus.Result.Equals(HolidayWfUtility.RejectResult))
                {
                    HolidayWfUtility.SetHoliday(taskStatus, HolidayStatus.R);

                    taskStatus.AddParameter(HolidayWfUtility.ResultParameter, HolidayWfUtility.RejectMessage);
                }
            }

            Result.Set(context, taskStatus);

            Log.Debug("OnCompleteMgrTask -> End");
        }

    }

    public sealed class OnCompleteUserTask : NativeActivity<TaskStateData>
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<TaskStateData> Request { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("OnCompleteUserTask -> Start");

            var taskStatus = WorkflowAction.GetTaskState(context, HolidayWfUtility.Holidayuser);

            WorkflowAction.SetWorkflowResult(context, "OK");

            Result.Set(context, taskStatus);

            Log.Debug("OnCompleteUserTask -> End");
        }

    }

    public class HolidayWfUtility
    {
        public static string Holidayhr = "HolidayHr";
        public static string Holidaymgr = "HolidayMgr";
        public static string Holidayuser = "HolidayUser";
        public static string RejectResult = "Reject";
        public static string RejectMessage = "Rejected";
        public static string ApproveMessage = "Approved";
        public static string ResultParameter = "HolidayResult";
        public static string UserParameter = "UserName";
        
        public static void SetHoliday(TaskStateData taskStatus, HolidayStatus res)
        {
            using (var proxy = new FlowTasksService())
            {
                proxy.UpdateHoliday(new UpdateHolidayRequest
                {
                    HolidayId = int.Parse(taskStatus.Parameters["HolidayId"]),
                    Status = res
                });
            }
        }
    }
}
