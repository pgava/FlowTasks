using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Flow.Tasks.Data.Core.Interfaces;
using Flow.Tasks.Data.Core;
using System.Text.RegularExpressions;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Flow.Users.Contract;
using Flow.Tasks.Data.Infrastructure;

namespace Flow.Tasks.Data.DAL
{
    /// <summary>
    /// Task
    /// </summary>
    public sealed class Task : ITask
    {
        /// <summary>
        /// Day Flag
        /// </summary>
        const string DAY_FLAG = "D";

        /// <summary>
        /// Hand Over Statuses
        /// </summary>
        public enum HandOverStatus
        {
            None,
            HandedOver,
            HandedBack
        }

        /// <summary>
        /// Tracer
        /// </summary>
        private readonly ITracer _tracer;

        /// <summary>
        /// FlowUsers Service
        /// </summary>
        private readonly IFlowUsersService _usersService;

        /// <summary>
        /// Notification Code
        /// </summary>
        private readonly string NotificationCode = "Notification";

        public Task(ITracer tracer)
        {
            _tracer = tracer;
        }

        public Task(IFlowUsersService usersService, ITracer tracer)
        {
            _usersService = usersService;
            _tracer = tracer;
        }

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="notificationInfo">notification Info</param>
        public void CreateNotification(Guid workflowOid, NotificationInfo notificationInfo)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                WorkflowDefinition wfd = uofw.WorkflowDefinitions.First(d => d.WorkflowOid == workflowOid, d => d.WorkflowCode);

                foreach (var u in ParseUsers.GetListUsersName(_usersService, wfd.Domain, notificationInfo.AssignedToUsers))
                {
                    CreateNotification(uofw, wfd, notificationInfo, u);
                }
            }
        }

        /// <summary>
        /// Check if task is a notification.
        /// </summary>
        /// <param name="taskOid"></param>
        /// <returns>Return true if Task is a notification</returns>
        public bool IsTaskNotification(Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid, t => t.WorkflowDefinition);
                if (tkd.TaskCode == NotificationCode) return true;
            }

            return false;
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="taskInfo">Task Info</param>
        /// <param name="properties">Properties</param>
        public void CreateTask(Guid workflowOid, TaskInfo taskInfo, IEnumerable<PropertyInfo> properties)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                WorkflowDefinition wfd = uofw.WorkflowDefinitions.First(d => d.WorkflowOid == workflowOid, d => d.WorkflowCode);

                // Check if there is a configuration that over writes initialization values
                ReadConfiguration(taskInfo, wfd.WorkflowCode);

                TaskDefinition tkd = new TaskDefinition
                    {
                        DefaultResult = taskInfo.DefaultResult,
                        UiCode = taskInfo.UiCode,
                        ExpiryDate = GetExpiryDatetime(taskInfo.ExpiresWhen, taskInfo.ExpiresIn),
                        TaskCode = taskInfo.TaskCode,
                        TaskOid = taskInfo.TaskOid,
                        WorkflowDefinition = wfd,
                        TaskCorrelationId = taskInfo.TaskCorrelationId
                    };

                SetParameters(uofw, wfd, tkd, properties);

                uofw.TaskDefinitions.Insert(tkd);

                // TODO: why do I need 2 commits?
                uofw.Commit();

                InitializePlaceHolders(uofw, tkd, taskInfo);
                InitializeUsers(uofw, tkd, taskInfo, wfd.Domain);

                uofw.Commit();
            }

            _tracer.Trace(workflowOid, ActionTrace.TaskCreated, taskInfo.TaskCode,
                string.Format(Properties.Resources.TK_STARTED, taskInfo.TaskCode), TraceEventType.Activity);

        }

        /// <summary>
        /// Complete Notification
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        public void CompleteNotification(Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid, t => t.WorkflowDefinition);
                uofw.TaskDefinitions.Delete(tkd);
                uofw.Commit();
            }
        }

        /// <summary>
        /// Complete Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <param name="result">Result</param>
        /// <param name="user">User</param>
        public void CompleteTask(Guid taskOid, string result, string user)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                // Leave notifications
                if (IsTaskNotification(taskOid)) return;

                TaskDefinition tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid, t => t.WorkflowDefinition);

                var wfc = uofw.WorkflowDefinitions.First(c => c.WorkflowDefinitionId == tkd.WorkflowDefinitionId, c => c.WorkflowCode);

                var tkp = uofw.TaskInParameters.Find(tp => tp.TaskDefinition.TaskOid == taskOid, tp => tp.Property, tp => tp.TaskDefinition)
                    .Select(tp => tp.Property).ToList();

                var tkwfProp = uofw.WorkflowProperties.Find(p => p.WorkflowCode.Code == wfc.WorkflowCode.Code, p => p.Property)
                    .Select(p => p.Property).ToList();

                var tkwfInParam = uofw.WorkflowInParameters.Find(p => p.WorkflowDefinitionId == tkd.WorkflowDefinitionId, p => p.Property)
                    .Select(p => p.Property).ToList();

                var tkwfOutParam = uofw.WorkflowOutParameters.Find(p => p.WorkflowDefinitionId == tkd.WorkflowDefinitionId, p => p.Property)
                    .Select(p => p.Property).ToList();

                DeleteProperties(uofw, tkp, tkwfProp, tkwfInParam, tkwfOutParam);

                _tracer.Trace(tkd.WorkflowDefinition.WorkflowOid,
                    ActionTrace.TaskCompleted, tkd.TaskCode, result, user,
                    string.Format(Properties.Resources.TK_COMPLETED, tkd.TaskCode, result),
                    TraceEventType.Activity);

                uofw.TaskDefinitions.Delete(tkd);
                uofw.Commit();
            }

        }

        /// <summary>
        /// Get Next Tasks For Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>Task Info</returns>
        public IEnumerable<TaskInfo> GetNextTasksForWorkflow(Guid workflowOid)
        {
            var inProgress = WorkflowStatusType.InProgress.ToString();

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var taskInfos = uofw.TaskDefinitions.Find(t => (t.WorkflowDefinition.WorkflowOid == workflowOid &&
                                    t.WorkflowDefinition.WorkflowStatus.Status == inProgress) ||
                                    (t.WorkflowDefinition.WorkflowParentDefinition.WorkflowOid == workflowOid &&
                                    t.WorkflowDefinition.WorkflowParentDefinition.WorkflowStatus.Status == inProgress), t => t.WorkflowDefinition)
                                    .Select(t => new TaskInfo
                                    {
                                        WorkflowOid = t.WorkflowDefinition.WorkflowOid,
                                        DefaultResult = t.DefaultResult,
                                        UiCode = t.UiCode,
                                        ExpiryDate = t.ExpiryDate,
                                        TaskCode = t.TaskCode,
                                        TaskOid = t.TaskOid,
                                        Title = t.Title,
                                        Description = t.Description,
                                        TaskCorrelationId = t.TaskCorrelationId,
                                        AcceptedBy = t.AcceptedBy
                                    }).ToList();

                return taskInfos;
            }
        }

        /// <summary>
        /// Get Next Tasks For User. 
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="domain">Domain</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchFor"></param>
        /// <returns>List of TaskInfo</returns>
        public IEnumerable<TaskInfo> GetNextTasksForUser(string user, Guid workflowOid, string domain, int pageIndex, int pageSize, string searchFor)
        {
            // TODO: hotfix to be removed
            if (pageSize == 0) pageSize = 10;

            IEnumerable<TaskInfo> taskInfos;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                taskInfos = NextTasksForUserQuery(uofw, user, workflowOid, domain, searchFor)
                                 .Select(t => new TaskInfo
                                 {
                                     WorkflowOid = t.WorkflowDefinition.WorkflowOid,
                                     DefaultResult = t.DefaultResult,
                                     UiCode = t.UiCode,
                                     ExpiryDate = t.ExpiryDate,
                                     TaskCode = t.TaskCode,
                                     TaskOid = t.TaskOid,
                                     Title = t.Title,
                                     Description = t.Description,
                                     TaskCorrelationId = t.TaskCorrelationId,
                                     AcceptedBy = t.AcceptedBy
                                 }).ToList();
            }


            var skip = pageIndex * pageSize;
            var tasks = taskInfos;

            var tasksForPage = tasks.Skip(skip).Take(pageSize).ToList();

            return tasksForPage;
        }

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="workflowOids">List of oid</param>
        /// <param name="domain">Domain</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchFor"></param>
        /// <returns>List of TaskInfo</returns>
        public IEnumerable<TaskInfo> GetNextTasksForUser(string user, IEnumerable<Guid> workflowOids, string domain, int pageIndex, int pageSize, string searchFor)
        {
            var tasks = new List<TaskInfo>();
            foreach (var oid in workflowOids)
            {
                tasks.AddRange(GetNextTasksForUser(user, oid, domain, pageIndex, pageSize, searchFor));
            }

            return tasks;
        }

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchFor"></param>
        /// <returns>List of TaskInfo</returns>
        public IEnumerable<TaskInfo> GetNextTasksForUser(string user, int pageIndex, int pageSize, string searchFor)
        {
            return GetNextTasksForUser(user, Guid.Empty, string.Empty, pageIndex, pageSize, searchFor);
        }

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="domain">Domain</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchFor"></param>
        /// <returns>List of TaskInfo</returns>
        public IEnumerable<TaskInfo> GetNextTasksForUser(string user, string domain, int pageIndex, int pageSize, string searchFor)
        {
            return GetNextTasksForUser(user, Guid.Empty, domain, pageIndex, pageSize, searchFor);
        }

        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="taskOid">TaskOid</param>
        /// <returns>TaskInfo</returns>
        public TaskInfo GetTask(Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var task = uofw.TaskDefinitions.Find(t => t.TaskOid == taskOid, t => t.WorkflowDefinition) 
                                 .Select(t => new TaskInfo
                                 {
                                     WorkflowOid = t.WorkflowDefinition.WorkflowOid,
                                     DefaultResult = t.DefaultResult,
                                     UiCode = t.UiCode,
                                     ExpiryDate = t.ExpiryDate,
                                     TaskCode = t.TaskCode,
                                     TaskOid = t.TaskOid,
                                     Title = t.Title,
                                     Description = t.Description,
                                     TaskCorrelationId = t.TaskCorrelationId,
                                     AcceptedBy = t.AcceptedBy
                                 }).FirstOrDefault();
                return task;
            }
        }


        /// <summary>
        /// Get Parameters For Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of property</returns>
        public IEnumerable<PropertyInfo> GetTaskParameters(Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                return GetTaskParameters(uofw, taskOid);
            }
        }

        /// <summary>
        /// Search For Tasks
        /// </summary>
        /// <param name="taskCode">TaskCode</param>
        /// <param name="acceptedBy">AcceptedBy</param>
        /// <param name="properties">Properties</param>
        /// <returns></returns>
        public IEnumerable<TaskInfo> SearchForTasks(string taskCode, string acceptedBy, IEnumerable<PropertyInfo> properties)
        {
            var matchProperties = properties as IList<PropertyInfo> ?? properties.ToList();
            if (string.IsNullOrWhiteSpace(taskCode) || properties == null || !matchProperties.Any())
            {
                throw new ArgumentException("TaskCode and Properties must be defined.");
            }

            List<TaskInfo> taskInfos = new List<TaskInfo>();
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var tds = uofw.TaskDefinitions.Find(td => td.TaskCode.Equals(taskCode, StringComparison.OrdinalIgnoreCase) &&
                    td.CompletedOn == null && (string.IsNullOrEmpty(acceptedBy) || td.AcceptedBy.Equals(acceptedBy, StringComparison.OrdinalIgnoreCase)), td => td.WorkflowDefinition).ToList();

                foreach (var t in tds)
                {
                    var taskParams = GetTaskParameters(uofw, t.TaskOid);
                    if (HasParameters(taskParams, matchProperties))
                    {
                        taskInfos.Add(new TaskInfo
                        {
                            WorkflowOid = t.WorkflowDefinition.WorkflowOid,
                            DefaultResult = t.DefaultResult,
                            UiCode = t.UiCode,
                            ExpiryDate = t.ExpiryDate,
                            TaskCode = t.TaskCode,
                            TaskOid = t.TaskOid,
                            Title = t.Title,
                            Description = t.Description,
                            TaskCorrelationId = t.TaskCorrelationId,
                            AcceptedBy = t.AcceptedBy
                        });
                    }
                }

                return taskInfos;
            }
        }

        /// <summary>
        /// Assign Task To User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="taskOid">Task Oid</param>
        public void AssignTaskTo(string user, Guid taskOid)
        {
            TaskDefinition tkd;
            TaskUserHandOver tkuh = null;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid, t => t.WorkflowDefinition);

                TaskUser tku = uofw.TaskUsers.FirstOrDefault(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == user);
                if (tku == null)
                {
                    tkuh = uofw.TaskUserHandOvers.First(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == user);
                }

                // If task already assigned to user don't do anything.
                if (!string.IsNullOrWhiteSpace(tkd.AcceptedBy) &&
                    tkd.AcceptedBy == (tku != null ? tku.User : tkuh.User)) return;

                tkd.AcceptedOn = DateTime.Now;
                tkd.AcceptedBy = (tku != null ? tku.User : tkuh.User);
                if (tku != null)
                {
                    tku.InUse = true;
                }
                else
                {
                    tkuh.InUse = true;
                }

                uofw.Commit();
            }

            _tracer.Trace(tkd.WorkflowDefinition.WorkflowOid, ActionTrace.TaskAssigned, tkd.TaskCode, string.Empty, tkd.AcceptedBy,
                string.Format(Properties.Resources.TK_ASSIGNED, tkd.TaskCode, tkd.AcceptedBy), TraceEventType.Activity);

        }

        /// <summary>
        /// Give Back Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        public void GiveBackTask(Guid taskOid)
        {
            TaskDefinition tkd;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid, t => t.WorkflowDefinition);

                tkd.HandedOverStatus = HandOverStatus.None.ToString();

                var tku = uofw.TaskUsers.FirstOrDefault(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == tkd.AcceptedBy);
                if (tku != null)
                {
                    tku.InUse = false;
                }
                else
                {
                    var tkuh = uofw.TaskUserHandOvers.FirstOrDefault(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == tkd.AcceptedBy);
                    if (tkuh != null)
                    {
                        tkuh.InUse = false;
                    }
                }

                tkd.AcceptedBy = string.Empty;
                tkd.AcceptedOn = null;

                uofw.Commit();
            }

            _tracer.Trace(tkd.WorkflowDefinition.WorkflowOid, ActionTrace.TaskGaveBack, tkd.TaskCode,
                string.Format(Properties.Resources.TK_GAVEBACK, tkd.TaskCode), TraceEventType.Activity);
        }

        /// <summary>
        /// Hand Over Task To User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="taskOid">Task Oid</param>
        public void HandOverTaskTo(string user, Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid);

                var tku = uofw.TaskUserHandOvers.First(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.User == user);

                tkd.HandedOverStatus = GetNextStatus(tkd.HandedOverStatus);
                tkd.AcceptedBy = string.Empty;
                tkd.AcceptedOn = null;
                tku.InUse = true;

                uofw.Commit();
            }
        }

        /// <summary>
        /// Get Users For Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of user name</returns>
        public IEnumerable<string> GetUsersForTask(Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid);

                if (string.IsNullOrWhiteSpace(tkd.HandedOverStatus) || tkd.HandedOverStatus == HandOverStatus.None.ToString())
                {
                    var tku = uofw.TaskUsers.Find(u => u.TaskDefinitionId == tkd.TaskDefinitionId);
                    foreach (var u in tku)
                    {
                        yield return u.User;
                    }
                }
                else if (tkd.HandedOverStatus == HandOverStatus.HandedBack.ToString())
                {
                    var tku = uofw.TaskUsers.Find(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.InUse);
                    foreach (var u in tku)
                    {
                        yield return u.User;
                    }
                }
                else if (tkd.HandedOverStatus == HandOverStatus.HandedOver.ToString())
                {
                    var tku = uofw.TaskUserHandOvers.Find(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.InUse);
                    foreach (var u in tku)
                    {
                        yield return u.User;
                    }
                }
            }
        }

        /// <summary>
        /// Get Hand Over Users For Task
        /// </summary>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of user name</returns>
        public IEnumerable<string> GetHandOverUsersForTask(Guid taskOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid);
                if (string.IsNullOrWhiteSpace(tkd.HandedOverStatus) || tkd.HandedOverStatus == HandOverStatus.None.ToString())
                {
                    var tku = uofw.TaskUserHandOvers.Find(u => u.TaskDefinitionId == tkd.TaskDefinitionId);
                    foreach (var u in tku)
                    {
                        yield return u.User;
                    }
                }
                else if (tkd.HandedOverStatus == HandOverStatus.HandedBack.ToString())
                {
                    var tku = uofw.TaskUserHandOvers.Find(u => u.TaskDefinitionId == tkd.TaskDefinitionId);
                    foreach (var u in tku)
                    {
                        yield return u.User;
                    }
                }
                else if (tkd.HandedOverStatus == HandOverStatus.HandedOver.ToString())
                {
                    var tku = uofw.TaskUsers.Find(u => u.TaskDefinitionId == tkd.TaskDefinitionId && u.InUse);
                    foreach (var u in tku)
                    {
                        yield return u.User;
                    }
                }
            }
        }

        /// <summary>
        /// Get Expiry in TimeSpan Format
        /// </summary>
        /// <param name="expiresWhen">Expires When</param>
        /// <param name="expiresIn">Expires In</param>
        /// <returns>Expiry TimeSpan</returns>
        public TimeSpan GetExpiryTimeSpan(DateTime? expiresWhen, string expiresIn)
        {
            if (expiresWhen.HasValue)
            {
                return expiresWhen.Value - DateTime.Now;
            }

            if (!string.IsNullOrWhiteSpace(expiresIn))
            {
                Match match = TranslateExpiresIn(expiresIn);

                if (match.Success)
                {
                    if (match.Groups[2].ToString().Equals(DAY_FLAG, StringComparison.OrdinalIgnoreCase))
                    {
                        return TimeSpan.FromDays(Double.Parse(match.Groups[1].ToString()));
                    }
                    return TimeSpan.FromMinutes(Double.Parse(match.Groups[1].ToString()));
                }
            }

            return TimeSpan.MaxValue;
        }

        /// <summary>
        /// Get Expiry in Datetime Format
        /// </summary>
        /// <param name="expiresWhen">Expires When</param>
        /// <param name="expiresIn">Expires In</param>
        /// <returns></returns>
        public DateTime? GetExpiryDatetime(DateTime? expiresWhen, string expiresIn)
        {
            if (expiresWhen.HasValue)
            {
                return expiresWhen.Value;
            }

            if (!string.IsNullOrWhiteSpace(expiresIn))
            {
                Match match = TranslateExpiresIn(expiresIn);

                if (match.Success)
                {
                    if (match.Groups[2].ToString().Equals(DAY_FLAG, StringComparison.OrdinalIgnoreCase))
                    {
                        return DateTime.Now.AddDays(Double.Parse(match.Groups[1].ToString()));
                    }
                    return DateTime.Now.AddMinutes(Double.Parse(match.Groups[1].ToString()));
                }
            }

            return null;
        }

        /// <summary>
        /// Get Task Count
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Number of tasks for user</returns>
        public int GetTaskCount(string user)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var q = NextTasksForUserQuery(uofw, user, Guid.Empty, string.Empty, string.Empty);
                return q.Count();
            }
        }

        /// <summary>
        /// Get Tasks Completed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="total"></param>
        /// <returns>list of TasksOn</returns>
        public IEnumerable<TasksOn> GetTasksCompleted(string user, out int total)
        {
            var results = new List<TasksOn>();
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var action = ActionTrace.TaskCompleted.ToString();
                var last = DateTime.Now.AddDays(-7);
                var taskDone = uofw.WorkflowTraces.Find(t => t.User.Equals(user, StringComparison.OrdinalIgnoreCase) &&
                                              t.Action == action &&
                                              t.When >= last).ToList();

                if (taskDone.Any())
                {
                    results = (from t in taskDone
                               group t.Code by t.When.ToString("ddd d MMM") into g
                               select new TasksOn { Date = g.Key, Counter = g.Count() }).ToList();
                }
                total = taskDone.Count;
            }


            return results;

        }

        /// <summary>
        /// Get Tasks To Do
        /// </summary>
        /// <param name="user"></param>
        /// <param name="total"></param>
        /// <returns>list of TasksOn</returns>
        public IEnumerable<TasksOn> GetTasksToDo(string user, out int total)
        {
            var tasks = GetNextTasksForUser(user, string.Empty, 0, 9999, string.Empty).ToList();

            var results = new List<TasksOn>();
            if (tasks.Any())
            {
                results = (from t in tasks
                              group t.TaskOid by t.ExpiryDate.HasValue ? t.ExpiryDate.Value.ToString("ddd d MMM") :  "Other" into g
                               select new TasksOn { Date = g.Key, Counter = g.Count() }).ToList();
            }
            total = tasks.Count;

            return results;
        }

        #region Private Methods

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="wfd">Workflow Definition</param>
        /// <param name="notificationInfo">Notification Info</param>
        /// <param name="user">User</param>
        private void CreateNotification(IFlowTasksUnitOfWork uofw, WorkflowDefinition wfd, NotificationInfo notificationInfo, string user)
        {
            TaskDefinition tkd;

            tkd = new TaskDefinition
            {
                UiCode = "ReadNotification",
                TaskCode = NotificationCode,
                TaskOid = notificationInfo.TaskOid,
                WorkflowDefinition = wfd,
                Title = notificationInfo.Title,
                Description = notificationInfo.Description,
                AcceptedBy = user,
                DefaultResult = "OK"
            };
            uofw.TaskDefinitions.Insert(tkd);

            // not implemented yet. do we need it?
            //InitializePlaceHolders(uofw, tkd, notificationInfo);

            InitializeNotificationToUsers(uofw, tkd, user);

            uofw.Commit();
        }

        /// <summary>
        /// Get Task Parameters
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="taskOid">Task Oid</param>
        /// <returns>List of PropertyInfo</returns>
        private IEnumerable<PropertyInfo> GetTaskParameters(IFlowTasksUnitOfWork uofw, Guid taskOid)
        {
            return uofw.TaskInParameters.Find(tp => tp.TaskDefinition.TaskOid == taskOid, tp => tp.Property)
                .Select(tp => new PropertyInfo { Name = tp.Property.Name, Value = tp.Property.Value, Type = tp.Property.Type }).ToList();
        }

        /// <summary>
        /// Translate Expires In
        /// </summary>
        /// <param name="expiresIn">ExpiresIn</param>
        /// <returns>Match</returns>
        private Match TranslateExpiresIn(string expiresIn)
        {
            return Regex.Match(expiresIn, @"^(\d*)([d,m]?)$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Get Next Status
        /// </summary>
        /// <param name="prevStatus"></param>
        /// <returns>Status</returns>
        private string GetNextStatus(string prevStatus)
        {
            if (string.IsNullOrWhiteSpace(prevStatus) || prevStatus == HandOverStatus.None.ToString())
            {
                return HandOverStatus.HandedOver.ToString();
            }
            if (prevStatus == HandOverStatus.HandedOver.ToString())
            {
                return HandOverStatus.HandedOver.ToString();
            }
            return HandOverStatus.HandedOver.ToString();
        }

        /// <summary>
        /// Read Configuration
        /// </summary>
        /// <param name="taskInfo">Task Info</param>
        /// <param name="workflowCode">Workflow Code</param>
        private void ReadConfiguration(TaskInfo taskInfo, WorkflowCode workflowCode)
        {
            TaskConfiguration tkc;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                tkc = uofw.TaskConfigurations.FirstOrDefault(c => c.WorkflowCode.Code == workflowCode.Code && c.TaskCode == taskInfo.TaskCode);
            }
            if (tkc == null) return;

            taskInfo.Title = string.IsNullOrWhiteSpace(tkc.Title) ? taskInfo.Title : tkc.Title;
            taskInfo.Title = string.IsNullOrWhiteSpace(tkc.Description) ? taskInfo.Description : tkc.Description;
            taskInfo.Title = string.IsNullOrWhiteSpace(tkc.AssignedToUsers) ? taskInfo.AssignedToUsers : tkc.AssignedToUsers;
            taskInfo.Title = string.IsNullOrWhiteSpace(tkc.HandOverUsers) ? taskInfo.HandOverUsers : tkc.HandOverUsers;
            taskInfo.Title = string.IsNullOrWhiteSpace(tkc.UiCode) ? taskInfo.UiCode : tkc.UiCode;
            taskInfo.Title = string.IsNullOrWhiteSpace(tkc.DefaultResult) ? taskInfo.DefaultResult : tkc.DefaultResult;
        }

        /// <summary>
        /// Initialize Users
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="tkd">Task Definition</param>
        /// <param name="taskInfo">Task Info</param>
        /// <param name="domain">Domain</param>
        private void InitializeUsers(IFlowTasksUnitOfWork uofw, TaskDefinition tkd, TaskInfo taskInfo, string domain)
        {
            bool foundUser = false;
            foreach (var u in ParseUsers.GetListUsersName(_usersService, domain, taskInfo.AssignedToUsers))
            {
                uofw.TaskUsers.Insert(CreateTaskUser(tkd, u));
                foundUser = true;
            }

            foreach (var u in ParseUsers.GetListUsersName(_usersService, domain, taskInfo.HandOverUsers))
            {
                uofw.TaskUserHandOvers.Insert(CreateTaskUserHandHover(tkd, u));
                foundUser = true;
            }

            if (!foundUser)
                throw new Exception("No valid user found");
        }

        /// <summary>
        /// Initialize Notification To Users
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="tkd">Task Definition</param>
        /// <param name="user">User</param>
        private void InitializeNotificationToUsers(IFlowTasksUnitOfWork uofw, TaskDefinition tkd, string user)
        {
            uofw.TaskUsers.Insert(CreateTaskUser(tkd, user));
        }

        /// <summary>
        /// Create Task User HandHover
        /// </summary>
        /// <param name="tkd">Task definition</param>
        /// <param name="u">User name</param>
        /// <returns>TaskUserHandOver</returns>
        private TaskUserHandOver CreateTaskUserHandHover(TaskDefinition tkd, string u)
        {
            return new TaskUserHandOver
            {
                User = u,
                TaskDefinition = tkd,
                InUse = false
            };
        }

        /// <summary>
        /// Create Task User
        /// </summary>
        /// <param name="tkd">Task Definition</param>
        /// <param name="u">User</param>
        /// <returns>TaskUser</returns>
        private TaskUser CreateTaskUser(TaskDefinition tkd, string u)
        {
            return new TaskUser
            {
                User = u,
                TaskDefinition = tkd,
                InUse = false
            };
        }

        /// <summary>
        /// Initialize Place Holders
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="tkd">Task Definition</param>
        /// <param name="taskInfo">Task Info</param>
        private void InitializePlaceHolders(IFlowTasksUnitOfWork uofw, TaskDefinition tkd, TaskInfo taskInfo)
        {
            var parameters = GetTaskParameters(uofw, taskInfo.TaskOid);
            var propertyInfos = parameters.ToList();
            tkd.Title = PlaceHolder.ReplacePlaceHolderParameter(taskInfo.Title, propertyInfos);
            tkd.Description = PlaceHolder.ReplacePlaceHolderParameter(taskInfo.Description, propertyInfos);
        }

        /// <summary>
        /// Set Parameters
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="workflowDefinition">Workflow Definition</param>
        /// <param name="taskDefinition">Task Definition</param>
        /// <param name="properties">Properties</param>
        private void SetParameters(IFlowTasksUnitOfWork uofw, WorkflowDefinition workflowDefinition, TaskDefinition taskDefinition, IEnumerable<PropertyInfo> properties)
        {
            // Read pre-defined workflow parameters
            var workflowParameters = uofw.WorkflowInParameters.Find(wp => wp.WorkflowDefinition.WorkflowDefinitionId == workflowDefinition.WorkflowDefinitionId,
                wp => wp.Property);

            // Read pre-defined task properties
            var taskProperties = uofw.TaskProperties.Find(tp => tp.WorkflowCode.Code == workflowDefinition.WorkflowCode.Code &&
                                    tp.TaskCode == taskDefinition.TaskCode, tp => tp.Property);

            // Keep track of property added
            var parametersAdded = new List<string>();

            // Add "global" task properties
            foreach (var p in taskProperties)
            {
                uofw.TaskInParameters.Insert(
                    CreateTaskInParameter(p.Property, taskDefinition));

                parametersAdded.Add(p.Property.Name);
            }

            // Add "global" workflow parameters
            foreach (var p in workflowParameters)
            {
                if (parametersAdded.Contains(p.Property.Name)) continue;

                uofw.TaskInParameters.Insert(
                    CreateTaskInParameter(p.Property, taskDefinition));

                parametersAdded.Add(p.Property.Name);
            }

            // Add "user" task properties
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    if (parametersAdded.Contains(p.Name)) continue;

                    uofw.TaskInParameters.Insert(
                        CreateTaskInParameter(
                        CreateProperty(p.Name, p.Value, p.Type), taskDefinition));

                    parametersAdded.Add(p.Name);
                }
            }

        }

        /// <summary>
        /// Delete Properties
        /// </summary>
        /// <param name="uofw">Unit of Work</param>
        /// <param name="tkp">Task Properties</param>
        /// <param name="tkwfProp">Workflow Properties</param>
        /// <param name="tkwfInParam">Workflow In Parameters</param>
        /// <param name="tkwfOutParam">Workflow Out Parameters</param>
        private void DeleteProperties(IFlowTasksUnitOfWork uofw, List<Property> tkp, List<Property> tkwfProp, List<Property> tkwfInParam, List<Property> tkwfOutParam)
        {
            // Delete all the property a part from the ones in :
            // WorkflowProperties and WorkflowInParameters and WorkflowOutParameters
            // I need those properties when I want to manualy restart
            // the workflow
            foreach (var p in tkp)
            {
                if (!tkwfProp.Contains(p) && !tkwfInParam.Contains(p) && !tkwfOutParam.Contains(p))
                {
                    uofw.Properties.Delete(p);
                }
            }
        }

        /// <summary>
        /// Create Task In Parameter
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="taskDefinition">TaskDefinition</param>
        /// <returns></returns>
        private TaskInParameter CreateTaskInParameter(Property property, TaskDefinition taskDefinition)
        {
            return new TaskInParameter
            {
                Property = property,
                TaskDefinition = taskDefinition
            };
        }

        /// <summary>
        /// Create Property
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="type">Type</param>
        /// <returns>Property</returns>
        private Property CreateProperty(string name, string value, string type)
        {
            return new Property
            {
                Name = name,
                Value = value,
                Type = type
            };
        }

        /// <summary>
        /// HasParameters
        /// </summary>
        /// <remarks>
        /// This checks whether there are any elements in matchProperties which aren't in sourceProperties - and then inverts the result.
        /// </remarks>
        /// <param name="sourceProperties"></param>
        /// <param name="matchProperties"></param>
        /// <returns></returns>
        private bool HasParameters(IEnumerable<PropertyInfo> sourceProperties, IEnumerable<PropertyInfo> matchProperties)
        {
            var found = true;
            var propertyInfos = sourceProperties as IList<PropertyInfo> ?? sourceProperties.ToList();
            foreach (var matchp in matchProperties)
            {
                if (!propertyInfos.Any(p => p.Name.Equals(matchp.Name,StringComparison.OrdinalIgnoreCase) &&
                    p.Value.Equals(matchp.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    found = false;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Next Tasks For User Query
        /// </summary>
        /// <param name="uofw">Uofw</param>
        /// <param name="user">User</param>
        /// <param name="workflowOid">WorkflowOid</param>
        /// <param name="domain">Domain</param>
        /// <param name="searchFor">SearchFor</param>
        /// <returns></returns>
        private IQueryable<TaskDefinition> NextTasksForUserQuery(IFlowTasksUnitOfWork uofw, string user, Guid workflowOid, string domain, string searchFor)
        {
            var handOverNone = HandOverStatus.None.ToString();
            var handOverBack = HandOverStatus.HandedBack.ToString();
            var handOverOver = HandOverStatus.HandedOver.ToString();

            var q = from t in uofw.TaskDefinitions.AsQueryable()
                    .Include("WorkflowDefinition")
                    join tu in uofw.TaskUsers.AsQueryable() on new { JoinProperty1 = t.TaskDefinitionId, JoinProperty2 = user } equals new { JoinProperty1 = tu.TaskDefinitionId, JoinProperty2 = tu.User } into tug
                    from lotu in tug.DefaultIfEmpty()
                    join tuh in uofw.TaskUserHandOvers.AsQueryable() on new { JoinProperty1 = t.TaskDefinitionId, JoinProperty2 = user } equals new { JoinProperty1 = tuh.TaskDefinitionId, JoinProperty2 = tuh.User } into tuhg
                    from lotuh in tuhg.DefaultIfEmpty()
                    where (workflowOid == Guid.Empty || t.WorkflowDefinition.WorkflowOid == workflowOid) &&
                         (string.IsNullOrEmpty(domain) || t.WorkflowDefinition.Domain == domain) &&
                         (string.IsNullOrEmpty(searchFor) || t.Title.Contains(searchFor)) &&
                         (string.IsNullOrEmpty(t.AcceptedBy) || t.AcceptedBy.Equals(user, StringComparison.OrdinalIgnoreCase)) &&
                         (
                            ((string.IsNullOrEmpty(t.HandedOverStatus) || t.HandedOverStatus.Equals(handOverNone, StringComparison.OrdinalIgnoreCase)) && lotu.User.Equals(user, StringComparison.OrdinalIgnoreCase)) ||
                            (t.HandedOverStatus.Equals(handOverBack, StringComparison.OrdinalIgnoreCase) && lotu.InUse && lotu.User.Equals(user, StringComparison.OrdinalIgnoreCase)) ||
                            (t.HandedOverStatus.Equals(handOverOver, StringComparison.OrdinalIgnoreCase) && lotuh.InUse && lotuh.User.Equals(user, StringComparison.OrdinalIgnoreCase))
                         )
                    select t;

            return q;

        }

        #endregion
    }
}

