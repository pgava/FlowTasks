using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using Flow.Tasks.Data.Core.Interfaces;
using Flow.Tasks.Data.Core;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Flow.Users.Contract.Message;
using Flow.Tasks.Data.Infrastructure;
using Flow.Library;
using Flow.Users.Contract;
using Flow.Users.Proxy;

namespace Flow.Tasks.Data.DAL
{
    /// <summary>
    /// Workflow
    /// </summary>
    public sealed class Workflow : IWorkflow
    {
        /// <summary>
        /// Tracer
        /// </summary>
        private readonly ITracer _tracer;

        /// <summary>
        /// Task
        /// </summary>
        private readonly ITask _task;

        /// <summary>
        /// FlowUsers Service
        /// </summary>
        private readonly IFlowUsersService _usersService;

        public Workflow(ITask task, ITracer tracer)
        {
            _tracer = tracer;
            _task = task;
        }

        public Workflow(ITask task, ITracer tracer, IFlowUsersService usersService)
        {
            _tracer = tracer;
            _task = task;
            _usersService = usersService;
        }
      
        /// <summary>
        /// Create a workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="parentWorkflowOid">Parent workflow oid</param>
        /// <param name="workflowCode">Workflow Code</param>
        /// <param name="domain">Workflow domain</param>
        /// <param name="properties">Workflow list of properties</param>
        public void CreateWorkflow(Guid workflowOid, Guid parentWorkflowOid, string workflowCode, string domain, IEnumerable<PropertyInfo> properties)
        {

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfc = uofw.WorkflowCodes.First(c => c.Code == workflowCode);

                var status = WorkflowStatusType.InProgress.ToString();
                var wfs = uofw.WorkflowStatuses.First(s => s.Status == status);

                WorkflowDefinition parentWfd = null;
                if (parentWorkflowOid != Guid.Empty)
                {
                    parentWfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == parentWorkflowOid);
                }

                if (string.IsNullOrWhiteSpace(domain))
                {
                    domain = Defaults.CommonWorkGroup;
                }

                var wfd = new WorkflowDefinition
                {
                    WorkflowOid = workflowOid,
                    Domain = domain,
                    WorkflowCode = wfc,
                    StartedOn = DateTime.Now,
                    WorkflowStatus = wfs,
                    WorkflowParentDefinition = parentWfd
                };
                uofw.WorkflowDefinitions.Insert(wfd);

                // Add "default" properties based on the specified domain
                AddProperties(uofw, domain, wfc, wfd, properties);

                uofw.Commit();
            }

            _tracer.Trace(workflowOid, ActionTrace.WorkflowCreated, workflowCode, 
                Properties.Resources.WF_STARTED, TraceEventType.Activity);
        }

        /// <summary>
        /// Add Workflow
        /// </summary>
        /// <param name="workflowCode">WorkflowCode</param>
        /// <param name="serviceUrl">ServiceUrl</param>
        /// <param name="bindingConfiguration">BindingConfiguration</param>
        /// <param name="serviceEndpoint">ServiceEndpoint</param>
        public void AddWorkflow(string workflowCode, string serviceUrl, string bindingConfiguration, string serviceEndpoint)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfc = uofw.WorkflowCodes.FirstOrDefault(wc => wc.Code == workflowCode);
                if (wfc == null)
                {
                    wfc = new WorkflowCode { Code = workflowCode, Description = "Added by Skecth" };
                    uofw.WorkflowCodes.Insert(wfc);
                }
                else
                {
                    var wfcfg = uofw.WorkflowConfigurations.FirstOrDefault(w => w.WorkflowCode.Code == wfc.Code && w.ExpiryDate == null, w => w.WorkflowCode);
                    if (wfcfg != null)
                    {
                        wfcfg.ExpiryDate = DateTime.Now;
                    }
                }

                var newcfg = new WorkflowConfiguration
                {
                    WorkflowCode = wfc,
                    ServiceUrl = serviceUrl,
                    BindingConfiguration = bindingConfiguration,
                    ServiceEndpoint = serviceEndpoint,
                    EffectiveDate = DateTime.Now
                };

                uofw.WorkflowConfigurations.Insert(newcfg);

                uofw.Commit();
            }
        }

        /// <summary>
        /// Update Workflow Parameters
        /// </summary>
        /// <param name="workflowOid">WorkflowOid</param>
        /// <param name="taskOid">TaskOid</param>
        /// <param name="properties">Properties</param>
        public void UpdateWorkflowParameters(Guid workflowOid, Guid taskOid, IEnumerable<PropertyInfo> properties)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                WorkflowDefinition wfd = null;
                if (workflowOid != Guid.Empty)
                {
                    wfd = uofw.WorkflowDefinitions.FirstOrDefault(w => w.WorkflowOid == workflowOid);
                }
                else if (taskOid != Guid.Empty)
                {
                    wfd = uofw.TaskDefinitions.Find(t => t.TaskOid == taskOid, t => t.WorkflowDefinition).Select(t => t.WorkflowDefinition).FirstOrDefault();
                }

                if (wfd != null)
                {
                    var parms = uofw.WorkflowInParameters.Find(p => p.WorkflowDefinitionId == wfd.WorkflowDefinitionId, p => p.Property);
                    var propertyInfos = properties as IList<PropertyInfo> ?? properties.ToList();

                    foreach (var p in parms)
                    {
                        foreach (var newp in propertyInfos)
                        {
                            if (p.Property.Name == newp.Name)
                            {
                                p.Property.Value = newp.Value;                                
                            }
                        }
                    }

                    uofw.Commit();
                }
            }
        }

        /// <summary>
        /// Sketch Workflow
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="xamlxOid">XamlxOid</param>
        /// <param name="changedBy">ChangedBy</param>
        /// <param name="status">Status</param>
        public void SketchWorkflow(string name, Guid xamlxOid, string changedBy, SketchStatusType status)
        {
            var sketchStatusStr = status.ToString();

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var sketchStatus = uofw.SketchStatuses.FirstOrDefault(ss => ss.Status == sketchStatusStr);

                var sc = uofw.SketchConfigurations.FirstOrDefault(s => s.Name == name);
                if (sc == null)
                {
                    sc = new SketchConfiguration { Name = name, XamlxOid = xamlxOid, ChangedBy = changedBy, LastSavedOn = DateTime.Now };
                    if (sketchStatus != null)
                    {
                        sc.SketchStatus = sketchStatus;
                    }

                    uofw.SketchConfigurations.Insert(sc);
                }
                else
                {
                    if (sketchStatus != null)
                    {
                        sc.SketchStatus = sketchStatus;
                    }
                    sc.LastSavedOn = DateTime.Now;
                    if (xamlxOid != Guid.Empty)
                    {
                        sc.XamlxOid = xamlxOid;
                    }
                    sc.ChangedBy = changedBy;
                }
                
                uofw.Commit();
            }
        }

        /// <summary>
        /// Get Sketch For Statuses
        /// </summary>
        /// <param name="name"></param>
        /// <param name="statuses">Statuses</param>
        /// <returns>List of SketchInfo</returns>
        public IEnumerable<SketchInfo> GetSketchForFilter(string name, IEnumerable<SketchStatusType> statuses)
        {
            var sketchStatusStr = new List<string>();
            foreach (var s in statuses)
            {
                sketchStatusStr.Add(s.ToString());
            }

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var sketchs = uofw.SketchConfigurations.Find(sc => (string.IsNullOrEmpty(name) || sc.Name == name) && sketchStatusStr.Contains(sc.SketchStatus.Status), sc => sc.SketchStatus)
                    .Select(sc => new SketchInfo { Name = sc.Name, Status = sc.SketchStatus.Status, ChangedBy = sc.ChangedBy, XamlxOid = sc.XamlxOid });

                return sketchs.ToList();
            }
        }

        /// <summary>
        /// Delete workflow from the system
        /// </summary>
        /// <param name="workflowOid">Workflow oid</param>
        public void DeleteWorkflow(Guid workflowOid)
        {

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfd = uofw.WorkflowDefinitions.FirstOrDefault(w => w.WorkflowOid == workflowOid);
                
                if (wfd == null) return;

                // Delete all children first
                var wfdChilds = uofw.WorkflowDefinitions.Find(w => w.WorkflowParentDefinitionId == wfd.WorkflowDefinitionId);

                foreach (var w in wfdChilds)
                {
                    DeleteWorkflow(uofw, w);
                }

                DeleteWorkflow(uofw, wfd);

                uofw.Commit();
            }
        }

        /// <summary>
        /// Complete Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="workflowStatusType">Workflow status</param>
        /// <param name="result">Result</param>
        /// <param name="message">Custom message</param>
        public void CompleteWorkflow(Guid workflowOid, WorkflowStatusType workflowStatusType, string result, string message)
        {
            WorkflowDefinition wfd;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid, w => w.WorkflowStatus,
                    w => w.WorkflowCode);

                var status = workflowStatusType.ToString();
                var wfs = uofw.WorkflowStatuses.First(s => s.Status == status);

                // Only workflow in progress 
                if (wfd.WorkflowStatus.Status != WorkflowStatusType.InProgress.ToString())
                    return;

                // Only complete the children if status is terminated or aborted ie user intervention
                if (workflowStatusType != WorkflowStatusType.Completed)
                {
                    var wfdChilds = uofw.WorkflowDefinitions.Find(w => w.WorkflowParentDefinitionId == wfd.WorkflowDefinitionId);
                    
                    foreach (var child in wfdChilds)
                    {
                        CompleteWorkflow(child.WorkflowOid, workflowStatusType, result, string.Empty);
                    }
                }

                var tkd = uofw.TaskDefinitions.Find(t => t.WorkflowDefinition.WorkflowOid == workflowOid);
                foreach (var t in tkd)
                {
                    _task.CompleteTask(t.TaskOid, Properties.Resources.TASK_TERMINATED, string.Empty);
                }

#if DELETE_IN_PARAMETERS
                /*
                    if you use this you will not been able to restart a wf and bring it to
                    the existing step
                */
                var wfInParm = uofw.WorkflowInParameters.Find(wip => wip.WorkflowDefinitionId == wfd.WorkflowDefinitionId, p => p.Property)
                    .Select(wip => wip.Property).ToList();

                var wfProp = uofw.WorkflowProperties.Find(p => p.WorkflowCode.Code == wfd.WorkflowCode.Code, p => p.Property)
                    .Select(p => p.Property).ToList();

                var wfOutParam = uofw.WorkflowOutParameters.Find(p => p.WorkflowDefinitionId == wfd.WorkflowDefinitionId, p => p.Property)
                    .Select(p => p.Property).ToList();

                // Delete all the property a part from the ones in :
                // WorkflowProperties and WorkflowOutParameters
                // I need those properties when I want to manualy restart
                // the workflow
                foreach (var p in wfInParm)
                {
                    if (!wfProp.Contains(p) && !wfOutParam.Contains(p))
                    {
                        uofw.Properties.Delete(p);
                    }
                }
#endif
                wfd.WorkflowStatus = wfs;
                wfd.CompletedOn = DateTime.Now;

                uofw.Commit();
            }

            _tracer.Trace(workflowOid, ActionTrace.WorkflowCompleted, wfd.WorkflowCode.Code, 
                result, string.Empty, string.Format(Properties.Resources.WF_COMPLETED, 
                workflowStatusType.ToString() + (string.IsNullOrWhiteSpace(message) ? string.Empty : " - " + message)), TraceEventType.Activity);
        }

        /// <summary>
        /// Get workflow configuration
        /// </summary>
        /// <param name="workflowCodeOrId">Workflow Code or Workflow Oid</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>Workflow Configuration</returns>
        public WorkflowConfigurationInfo GetWorkflowConfiguration(string workflowCodeOrId, DateTime effectiveDate)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                Guid oid;
                bool isId = Guid.TryParse(workflowCodeOrId, out oid);

                string workflowCode;

                // we got an oid
                if (isId)
                {
                    workflowCode = uofw.WorkflowDefinitions.First(wfd => wfd.WorkflowOid == oid,
                        wfd => wfd.WorkflowCode).WorkflowCode.Code;
                }
                else
                {
                    workflowCode = workflowCodeOrId;
                }

                var wfc = uofw.WorkflowConfigurations.Find(c => c.WorkflowCode.Code == workflowCode && c.EffectiveDate <= effectiveDate &&
                           (!c.ExpiryDate.HasValue || c.ExpiryDate > effectiveDate)).Select(ci => new WorkflowConfigurationInfo
                           {
                               ServiceEndPoint = ci.ServiceEndpoint,
                               ServiceUrl = ci.ServiceUrl,
                               BindingConfiguration = ci.BindingConfiguration,
                               ServiceDefinition = ci.ServiceDefinition
                           }).First();

                return wfc;
            }
        }

        /// <summary>
        /// Get workflow parameters
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of properties</returns>
        public IEnumerable<PropertyInfo> GetWorkflowParameters(Guid workflowOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid);

                var wfParam = uofw.WorkflowInParameters.Find(p => p.WorkflowDefinitionId == wfd.WorkflowDefinitionId, p => p.Property)
                    .Select(p => p.Property);

                return wfParam.Select(wp => new PropertyInfo { Name = wp.Name, Value = wp.Value, Type = wp.Type }).ToList();
            }
        }

        /// <summary>
        /// Get Workflow output parameters
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of properties</returns>
        public IEnumerable<PropertyInfo> GetWorkflowOutParameters(Guid workflowOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid);

                var wfParam = uofw.WorkflowOutParameters.Find(p => p.WorkflowDefinitionId == wfd.WorkflowDefinitionId, p => p.Property)
                    .Select(p => p.Property);

                return wfParam.Select(wp => new PropertyInfo { Name = wp.Name, Value = wp.Value, Type = wp.Type }).ToList();
            }
        }

        /// <summary>
        /// Get workflow trace
        /// </summary>
        /// <param name="workflowOids">List of oid</param>
        /// <returns>List of trace</returns>
        public IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid[] workflowOids)
        {
            // TODO
            // The problem here is that we need to get the avatar for the user
            // who add comment to workflow, but the user information is in an
            // other database.
            // For now use a dictionary to make sure we don't call the service
            // twice for the same user.

            var userHash = new Dictionary<string, string>();

            var traces = _tracer.GetTraceForWorkflow(workflowOids).ToList();

                if (_usersService == null)
                {
                    using (var usersOperations = new FlowUsersService())
                    {
                        foreach (var t in traces)
                        {
                            if (string.IsNullOrWhiteSpace(t.User)) continue;

                            if (!userHash.ContainsKey(t.User))
                            {
                                var user = usersOperations.GetUser(new GetUserRequest {User = t.User});
                                if (user.User != null)
                                {
                                    userHash.Add(t.User, user.User.PhotoPath);
                                }
                            }

                            if (userHash.ContainsKey(t.User))
                            {
                                t.Avatar = userHash[t.User];
                            }
                        }
                    }
                }
                else
                {
                    foreach (var t in traces)
                    {
                        var user = _usersService.GetUser(new GetUserRequest {User = t.User});
                        t.Avatar = user.User.PhotoPath;
                    }
                }
            return traces;
        }

        /// <summary>
        /// Add trace to workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="taskOid">Task Oid</param>
        /// <param name="user">User</param>
        /// <param name="message">Message</param>
        public void AddTraceToWorkflow(Guid workflowOid, Guid taskOid, string user, string message)
        {
            _tracer.AddTraceToWorkflow(workflowOid, taskOid, user, message);
        }

        /// <summary>
        /// Check workflow is active
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>True if workflow is active</returns>
        public bool IsWorkflowInProgress(Guid workflowOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid, w => w.WorkflowStatus);

                return wfd.WorkflowStatus.Status == WorkflowStatusType.InProgress.ToString();
            }
        }

        /// <summary>
        /// Get workflow children
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of oid</returns>
        public IEnumerable<Guid> GetWorkflowChildren(Guid workflowOid)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid);

                var wfdChilds = uofw.WorkflowDefinitions.Find(w => w.WorkflowParentDefinitionId == wfd.WorkflowDefinitionId)
                    .Select(w => w.WorkflowOid);

                return wfdChilds.ToList();
            }
        }

        /// <summary>
        /// Get the workflow result. Status, Output parameters,...
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>Workflow result</returns>
        public WorkflowResultInfo GetWorkflowResult(Guid workflowOid)
        {
            WorkflowDefinition wfd;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid, w => w.WorkflowStatus);
            }

            var resultInfo = new WorkflowResultInfo
                                 {
                                     WorkflowOid = wfd.WorkflowOid,
                                     Status = wfd.WorkflowStatus.Status,
                                     CompletedOn = wfd.CompletedOn.HasValue ? wfd.CompletedOn.Value : DateTime.MinValue,
                                     OutParameters = new PropertyInfos(GetWorkflowOutParameters(workflowOid)),
                                     Result = string.Empty
                                 };

            // Get the result from Trace table. Only for completed workflows
            var traces = _tracer.GetTraceForWorkflow(new[] {workflowOid}, TraceEventType.Activity);
            foreach (var t in traces)
            {
                if (t.Action == ActionTrace.WorkflowCompleted.ToString())
                {
                    resultInfo.Result = t.Result;
                    break;
                }
            }

            return resultInfo;
        }

        /// <summary>
        /// Search Workflows
        /// </summary>
        /// <param name="workflowCode">WorkflowCode</param>
        /// <param name="domain">Domain</param>
        /// <param name="properties">Properties</param>
        /// <returns></returns>
        public IEnumerable<WorkflowInfo> SearchWorkflows(string workflowCode, string domain,
            IEnumerable<PropertyInfo> properties)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var inProgress = WorkflowStatusType.InProgress.ToString();

                var q = from wd in uofw.WorkflowDefinitions.AsQueryable()
                    where wd.WorkflowCode.Code.Equals(workflowCode) && wd.WorkflowStatus.Status.Equals(inProgress) &&
                          (string.IsNullOrWhiteSpace(domain) || domain == Library.Properties.Resources.SELECT_DEFAULT ||
                           wd.Domain.Equals(domain))
                    join wp in uofw.WorkflowInParameters.AsQueryable() on
                        wd.WorkflowDefinitionId equals wp.WorkflowDefinitionId
                    select new {WfDef = wd, WfParm = wp.Property};

                var res = new List<WorkflowInfo>();

                foreach (var w in q)
                {
                    var found = properties.All(p => w.WfParm.Name.Equals(p.Name) && w.WfParm.Value.Equals(p.Value));

                    if (found)
                    {
                        res.Add(new WorkflowInfo
                        {
                            WorkflowOid = w.WfDef.WorkflowOid,
                            WorkflowCode = w.WfDef.WorkflowCode.Code,
                            Domain = w.WfDef.Domain,
                            Status = w.WfDef.WorkflowStatus.Status,
                            ParentWorkflowOid = w.WfDef.WorkflowParentDefinition == null ? Guid.Empty : w.WfDef.WorkflowParentDefinition.WorkflowOid,
                            CompletedOnDate = w.WfDef.CompletedOn,
                            StartedOnDate = w.WfDef.StartedOn
                        });
                    }
                }

                return res;
            }

        }

        /// <summary>
        /// Get workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="workflowCode">Workflow Code</param>
        /// <param name="domain">Domain</param>
        /// <param name="isActive">Active?</param>
        /// <param name="startFrom">From</param>
        /// <param name="startTo">To</param>
        /// <param name="user">User</param>
        /// <param name="role">Role</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="workflowCodes"></param>
        /// <returns>List of WorkflowInfo</returns>
        public IEnumerable<WorkflowInfo> GetWorkflows(Guid workflowOid, string workflowCode, string domain, bool isActive, DateTime? startFrom, DateTime? startTo, 
            string user, string role, int pageIndex, int pageSize, out int total, out IEnumerable<string> workflowCodes)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                // Build the query based on the input filters
                var cond = PredicateBuilder.True<WorkflowDefinition>();

                if (workflowOid != Guid.Empty)
                {
                    cond = cond.And(w => w.WorkflowOid == workflowOid);
                }

                if (!string.IsNullOrWhiteSpace(workflowCode) && workflowCode != Library.Properties.Resources.SELECT_DEFAULT)
                {
                    cond = cond.And(w => w.WorkflowCode.Code == workflowCode);
                }

                if (!string.IsNullOrWhiteSpace(domain) && domain != Library.Properties.Resources.SELECT_DEFAULT)
                {
                    cond = cond.And(w => w.Domain == domain);
                }

                if (isActive)
                {
                    var inProgress = WorkflowStatusType.InProgress.ToString();
                    cond = cond.And(w => w.WorkflowStatus.Status == inProgress);
                }

                if (startFrom.HasValue)
                {
                    cond = cond.And(w => w.StartedOn >= startFrom.Value);
                }

                if (startTo.HasValue)
                {
                    cond = cond.And(w => w.StartedOn <= startTo.Value);
                }

                // Get the domains the user in on for the specified role
                var domains = GetDomainsForUser(user, role);
                var hasUser = !string.IsNullOrWhiteSpace(user);

                var skip = pageIndex > 0 ? (pageIndex - 1) * pageSize : 0;

                // Returns all the workflows that belongs to the user's domains
                var workflows = uofw.WorkflowDefinitions.Find(cond, w => w.WorkflowCode, w => w.WorkflowStatus)
                    .Where(wi => !hasUser || domains.Contains(wi.Domain))
                    .OrderBy(wi => wi.StartedOn)
                    .Select(wi => new WorkflowInfo
                {
                    WorkflowOid = wi.WorkflowOid,
                    WorkflowCode = wi.WorkflowCode.Code,
                    Domain = wi.Domain,
                    Status = wi.WorkflowStatus.Status,
                    ParentWorkflowOid = wi.WorkflowParentDefinition == null ? Guid.Empty : wi.WorkflowParentDefinition.WorkflowOid,
                    CompletedOnDate = wi.CompletedOn,
                    StartedOnDate = wi.StartedOn
                });

                // TODO : 
                // make sure to return the right number of workflow.
                // The problem is when there are chids workflows

                total = workflows.Count();
                workflowCodes = workflows.Select(w => w.WorkflowCode).Distinct().ToList();

                var workflowsInPage = workflows.OrderByDescending(w => w.StartedOnDate).Skip(skip).Take(pageSize).ToList();

                return workflowsInPage;
            }
        }

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        /// <returns>List of active WorkflowTypeInfo</returns>
        public IEnumerable<WorkflowTypeInfo> GetWorkflowType(DateTime effectiveDate)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfc = uofw.WorkflowConfigurations.Find(c => c.EffectiveDate <= effectiveDate &&
                          (!c.ExpiryDate.HasValue || c.ExpiryDate > effectiveDate), c => c.WorkflowCode)
                          .Select(wtf => new WorkflowTypeInfo
                          {
                              WorkflowCode = wtf.WorkflowCode.Code,
                              Description = wtf.WorkflowCode.Description
                          });

                return wfc.ToList();
            }
        }

        /// <summary>
        /// Report User Tasks
        /// </summary>
        /// <returns>List of User/Tasks</returns>
        public IEnumerable<ReportUserTasksInfo> ReportUserTasks(DateTime? start, DateTime? end, IEnumerable<string> users)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                return uofw.WorkflowTraces.ExecuteStoreQuery<ReportUserTasksInfo>("exec ReportUserTasks @p0, @p1", start, end)
                    .Where(q => users == null || !users.Any() || users.Contains(q.User))
                    .ToList();
            }
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <returns>List of Task/Duration</returns>
        public IEnumerable<ReportTaskTimeInfo> ReportTaskTime(DateTime? start, DateTime? end, IEnumerable<string> tasks, IEnumerable<string> workflows)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                return uofw.WorkflowTraces.ExecuteStoreQuery<ReportTaskTimeInfo>("exec ReportTaskTime @p0, @p1", start, end)
                    .Where(q => (tasks == null || !tasks.Any() || tasks.Contains(q.Task)) && (workflows == null || !workflows.Any() || workflows.Contains(q.Workflow)))
                    .ToList();
            }
        }

        /// <summary>
        /// Report Workflow Time
        /// </summary>
        /// <returns>List of Task/Duration</returns>
        public IEnumerable<ReportWorkflowTimeInfo> ReportWorkflowTime(DateTime? start, DateTime? end, IEnumerable<string> workflows)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                return uofw.WorkflowTraces.ExecuteStoreQuery<ReportWorkflowTimeInfo>("exec ReportWorkflowTime @p0, @p1", start, end)
                    .Where(q => workflows == null || !workflows.Any() || workflows.Contains(q.Workflow))
                    .ToList();
            }
        }

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <returns>List of Task/Duration</returns>
        public IEnumerable<ReportUserTaskCountInfo> ReportUserTaskCount(DateTime? start, DateTime? end, IEnumerable<string> users, IEnumerable<string> tasks)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                return uofw.WorkflowTraces.ExecuteStoreQuery<ReportUserTaskCountInfo>("exec ReportUserTaskCount @p0, @p1", start, end)
                    .Where(q => (tasks == null || !tasks.Any() || tasks.Contains(q.Task)) && (users == null || !users.Any() || users.Contains(q.User)))
                    .ToList();
            }
        }

        #region Private Methods

	/// <summary>
        /// Add Properties
	/// </summary>
	/// <param name="uofw">Unit of Work</param>
	/// <param name="domain">Domain</param>
	/// <param name="wfc">Workflow Code</param>
	/// <param name="wfd">Workflow Definition</param>
	/// <param name="properties">Properties</param>
        private void AddProperties(IFlowTasksUnitOfWork uofw, string domain, WorkflowCode wfc, WorkflowDefinition wfd, IEnumerable<PropertyInfo> properties)
        {
            var wfps = uofw.WorkflowProperties.Find(wfp => wfp.WorkflowCode.Code == wfc.Code &&
                       (wfp.Domain == domain || string.IsNullOrEmpty(wfp.Domain)), wfp => wfp.Property);

            foreach (var p in wfps)
            {
                uofw.WorkflowInParameters.Insert(
                    CreateWorkflowInParameter(
                        CreateProperty(p.Property.Name, p.Property.Value, p.Property.Type), wfd));
            }

            // Add properties from the parameter
            if (properties != null)
            {
                // Don't copy properties twice
                var workflowPropsName = wfps.Select(wfp => wfp.Property.Name);
                var distinctProp = properties.Where(p => !workflowPropsName.Contains(p.Name)).Select(p => p);

                foreach (var p in distinctProp)
                {
                    uofw.WorkflowInParameters.Insert(
                        CreateWorkflowInParameter(
                        CreateProperty(p.Name, p.Value, p.Type), wfd));
                }
            }
        }

	/// <summary>
        /// Delete Workflow
	/// </summary>
	/// <param name="uofw">Unit of Work</param>
	/// <param name="wfd">Workflow Definition</param>
        private void DeleteWorkflow(FlowTasksUnitOfWork uofw, WorkflowDefinition wfd)
        {
            if (wfd == null) return;

            uofw.WorkflowDefinitions.Delete(wfd);
        }

	/// <summary>
        /// Create Workflow InParameter
	/// </summary>
	/// <param name="property">Property</param>
        /// <param name="workflowdefinition">Workflow Definition</param>
        /// <returns>WorkflowInParameter</returns>
        private WorkflowInParameter CreateWorkflowInParameter(Property property, WorkflowDefinition workflowdefinition)
        {
            return new WorkflowInParameter
            {
                Property = property,
                WorkflowDefinition = workflowdefinition
            };
        }

	/// <summary>
        /// Create Property
	/// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="type">Type</param>
	/// <returns></returns>
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
        /// Get Domains For User
	/// </summary>
        /// <param name="user">User</param>
        /// <param name="role">Role</param>
	/// <returns>List of string</returns>
        private IEnumerable<string> GetDomainsForUser(string user, string role)
        {
            List<string> res = new List<string>();

            if (_usersService == null)
            {
                using (var usersOperations = new FlowUsersService())
                {
                    var domains = usersOperations.GetDomainsForUser(new GetDomainsForUserRequest { User = user, Role = role });
                    if (domains != null) res.AddRange(domains.Domains);
                }
            }
            else
            {
                var domains = _usersService.GetDomainsForUser(new GetDomainsForUserRequest { User = user, Role = role });
                if (domains != null) res.AddRange(domains.Domains);
            }

            return res;
        }

        #endregion
    }
}
