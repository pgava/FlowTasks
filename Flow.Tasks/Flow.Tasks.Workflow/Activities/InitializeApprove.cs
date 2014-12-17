using System;
using System.Collections.Generic;
using System.Activities;
using System.Reflection;
using log4net;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// InitializeApprove
    /// </summary>
    /// <remarks>
    /// Activity used to create a new task in the database
    /// </remarks>
    internal class InitializeApprove : NativeActivity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<string> TaskCode { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            Log.Debug("InitializeApprove -> Start");

            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus == null) throw new Exception("Cannot find WorkflowStateData");
            CreateTask(context, workflowStatus.Tasks[TaskCode.Get(context)].TaskInfo, workflowStatus.Tasks[TaskCode.Get(context)].Parameters);

            Log.Debug("InitializeApprove -> End");

        }

        /// <summary>
        /// Create task
        /// </summary>
        /// <param name="context">NativeActivityContext</param>
        /// <param name="taskInfo">TaskInfo</param>
        /// <param name="properties">Properties</param>
        private void CreateTask(NativeActivityContext context, TaskInfo taskInfo, Dictionary<string, string> properties)
        {
            Log.Debug("InitializeApprove -> Assigned users: " + taskInfo.AssignedToUsers);

            var taskOid = Guid.NewGuid();
            taskInfo.TaskOid = taskOid;

            var props = new List<Contract.Message.PropertyInfo>();
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    props.Add(new Contract.Message.PropertyInfo { Name = p.Key, Value = p.Value });
                }
            }
            
            using (var proxy = new Proxy.FlowTasksService())
            {
                proxy.CreateTask(new CreateTaskRequest
                {
                    WorkflowId = context.WorkflowInstanceId.ToString(),
                    TaskInfo = taskInfo,
                    Properties = new PropertyInfos(props)
                });
            }

        }

    }
}
