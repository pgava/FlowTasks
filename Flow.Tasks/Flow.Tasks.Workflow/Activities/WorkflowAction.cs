using Flow.Tasks.Contract.Message;
using Flow.Tasks.Proxy;
using System;
using System.Activities;

namespace Flow.Tasks.Workflow.Activities
{
    /// <summary>
    /// Workflow Action
    /// </summary>
    public sealed class WorkflowAction
    {
        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="workflowOid">WorkflowOid</param>
        /// <param name="taskOid">TaskOid</param>
        /// <param name="subject">Subject</param>
        /// <param name="message">Message</param>
        /// <param name="user">User. Support place holder</param>
        static public void SendNotification(Guid workflowOid, Guid taskOid, string subject, string message, string user)
        {
            using (var proxy = new FlowTasksService())
            {
                proxy.CreateNotification(new CreateNotificationRequest {
                    WorkflowId = workflowOid.ToString(),
                    NotificationInfo = new NotificationInfo {
                    AssignedToUsers = user,
                    Description = message,
                    Title = subject,
                    TaskOid = taskOid,
                    WorkflowOid = workflowOid
                    }
                });
            }
        }

        /// <summary>
        /// Get Task State from context
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="taskCode">TaskCode</param>
        /// <returns></returns>
        static public TaskStateData GetTaskState(NativeActivityContext context, string taskCode)
        {
            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus != null)
            {
                if (workflowStatus.Tasks.ContainsKey(taskCode))
                {
                    var taskData = workflowStatus.Tasks[taskCode];
                    if (workflowStatus.Parameters != null)
                    {
                        foreach (var p in workflowStatus.Parameters)
                        {
                            taskData.AddParameter(p.Key, p.Value);
                        }

                        // If you add parameters to task from the UI. The next line
                        // will remove them.
                        //taskData.Parameters = workflowStatus.Parameters;
                    }
                    return taskData;
                }
            }

            return null;
        }

        /// <summary>
        /// Set Workflow Result
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="result">Result</param>
        static public void SetWorkflowResult(NativeActivityContext context, string result)
        {
            var workflowStatus = context.Properties.Find(WorkflowStateData.Name) as WorkflowStateData;
            if (workflowStatus != null)
            {
                workflowStatus.Result = result;
            }
        }
    }
}
