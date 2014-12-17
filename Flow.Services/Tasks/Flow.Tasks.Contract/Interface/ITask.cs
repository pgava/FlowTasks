using System;
using System.Collections.Generic;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Contract.Interface
{
    /// <summary>
    /// Task Interface
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Assign Task To User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="taskOid">Task Oid</param>
        void AssignTaskTo(string user, Guid taskOid);

        /// <summary>
        /// Complete Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <param name="result">Result</param>
        /// <param name="user">User</param>
        void CompleteTask(Guid taskOid, string result, string user);


        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="taskInfo">Task Info</param>
        /// <param name="properties">Properties</param>
        void CreateTask(Guid workflowOid, TaskInfo taskInfo, IEnumerable<PropertyInfo> properties);

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="notificationInfo">notification Info</param>
        void CreateNotification(Guid workflowOid, NotificationInfo notificationInfo);

        /// <summary>
        /// Check if task is a notification.
        /// </summary>
        /// <param name="taskOid"></param>
        /// <returns>Return true if Task is a notification</returns>
        bool IsTaskNotification(Guid taskOid);

        /// <summary>
        /// Complete Notification
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        void CompleteNotification(Guid taskOid);

        /// <summary>
        /// Get Hand Over Users For Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of user name</returns>
        IEnumerable<string> GetHandOverUsersForTask(Guid taskOid);

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="workflowOid">List of oid</param>
        /// <param name="domain">Domain</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchFor"></param>
        /// <returns>List of TaskInfo</returns>
        IEnumerable<TaskInfo> GetNextTasksForUser(string user, Guid workflowOid, string domain, int pageIndex, int pageSize, string searchFor);

        /// <summary>
        /// Get Next Tasks For Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>Task Info</returns>
        IEnumerable<TaskInfo> GetNextTasksForWorkflow(Guid workflowOid);

        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="taskOid">TaskOid</param>
        /// <returns>Task Info</returns>
        TaskInfo GetTask(Guid taskOid);

        /// <summary>
        /// Get Parameters For Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of property</returns>
        IEnumerable<PropertyInfo> GetTaskParameters(Guid taskOid);

        /// <summary>
        /// Search For Tasks
        /// </summary>
        /// <param name="taskCode">TaskCode</param>
        /// <param name="acceptedBy">AcceptedBy</param>
        /// <param name="properties">Properties</param>
        /// <returns></returns>
        IEnumerable<TaskInfo> SearchForTasks(string taskCode, string acceptedBy, IEnumerable<PropertyInfo> properties);

        /// <summary>
        /// Get Users For Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of user name</returns>
        IEnumerable<string> GetUsersForTask(Guid taskOid);

        /// <summary>
        /// Give Back Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        void GiveBackTask(Guid taskOid);

        /// <summary>
        /// Hand Over Task To User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="taskOid">Task Oid</param>
        void HandOverTaskTo(string user, Guid taskOid);

        /// <summary>
        /// Get Expiry in TimeSpan Format
        /// </summary>
        /// <param name="expiresWhen">Expires When</param>
        /// <param name="expiresIn">Expires In</param>
        /// <returns>Expiry TimeSpan</returns>
        TimeSpan GetExpiryTimeSpan(DateTime? expiresWhen, string expiresIn);

        /// <summary>
        /// Get Expiry in Datetime Format
        /// </summary>
        /// <param name="expiresWhen">Expires When</param>
        /// <param name="expiresIn">Expires In</param>
        /// <returns></returns>
        DateTime? GetExpiryDatetime(DateTime? expiresWhen, string expiresIn);

        /// <summary>
        /// Get Task Count
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Number of tasks for user</returns>
        int GetTaskCount(string user);

        /// <summary>
        /// Get Tasks Completed
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="total"></param>
        /// <returns>List of TaskOn</returns>
        IEnumerable<TasksOn> GetTasksCompleted(string user, out int total);

        /// <summary>
        /// Get Tasks To Do
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="total"></param>
        /// <returns>List of TaskOn</returns>
        IEnumerable<TasksOn> GetTasksToDo(string user, out int total);


    }
}
