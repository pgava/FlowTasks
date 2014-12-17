using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Flow.Tasks.Data.Core;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Data.Infrastructure;

namespace Flow.Tasks.Data.DAL
{
    /// <summary>
    /// Tracer
    /// </summary>
    public sealed class Tracer : ITracer
    {
        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="action">Action</param>
        /// <param name="code">Code</param>
        /// <param name="msg">Msg</param>
        /// <param name="type">Type</param>
        public void Trace(Guid workflowOid, ActionTrace action, string code, string msg, TraceEventType type)
        {
            Trace(workflowOid, action, code, string.Empty, string.Empty, msg, type);
        }

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="action">Action</param>
        /// <param name="code">Code</param>
        /// <param name="result">Result</param>
        /// <param name="user">User</param>
        /// <param name="msg">Msg</param>
        /// <param name="type">Type</param>
        public void Trace(Guid workflowOid, ActionTrace action, string code, string result, string user, string msg, TraceEventType type)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfd = uofw.WorkflowDefinitions.First(w => w.WorkflowOid == workflowOid);

                var eventType = type.ToString();
                var tre = uofw.TraceEvents.First(e => e.Type == eventType);
                var t = new WorkflowTrace
                {
                    Message = msg.Substring(0, Math.Min(msg.Length, 500)),
                    TraceEvent = tre,
                    User = string.IsNullOrWhiteSpace(user) ? string.Empty : user,
                    When = DateTime.Now,
                    WorkflowDefinition = wfd,
                    Action = action.ToString(),
                    Code = code,
                    Result = result
                };

                uofw.WorkflowTraces.Insert(t);

                uofw.Commit();
            }
        }

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOids">List of Workflow Oid</param>
        /// <param name="type">Type</param>
        /// <returns>List of Trace</returns>
        public IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid[] workflowOids, TraceEventType type)
        {
            var traces = GetTraceForWorkflow(workflowOids);

            return traces.Where(t => t.Type == type.ToString()).OrderBy(o => OrderByDate(o.When));
        }

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOids">List of Workflow Oid</param>
        /// <returns>List of Trace</returns>
        public IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid[] workflowOids)
        {
            if (workflowOids == null || workflowOids.Count() == 0) throw new ArgumentException("workflowOids must have at least a value");

            var traceIntoList = new List<WorkflowTraceInfo>();
            foreach (var woid in workflowOids)
            {
                var traces = GetTraceForWorkflow(woid);

                traceIntoList.AddRange(traces);
            }

            return traceIntoList.OrderBy(o => OrderByDate(o.When));
        }

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOid"></param>
        /// <returns>List of WorkflowTraceInfo</returns>
        public IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid workflowOid)
        {
            return GetTraceForWorkflow(workflowOid, new List<WorkflowTraceInfo>()).OrderBy(o => OrderByDate(o.When));
        }

        /// <summary>
        /// Add Trace To Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="taskOid">Task Oid</param>
        /// <param name="user">User</param>
        /// <param name="message">Message</param>
        public void AddTraceToWorkflow(Guid workflowOid, Guid taskOid, string user, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            TaskDefinition tkd;
            using (var uofw = new FlowTasksUnitOfWork())
            {
                tkd = uofw.TaskDefinitions.First(t => t.TaskOid == taskOid);
            }

            Trace(workflowOid, ActionTrace.UserMessage, tkd.TaskCode, string.Empty,
                user, message, TraceEventType.Info);
        }

        #region Private Methods

        /// <summary>
        /// Order By Date
        /// </summary>
        /// <param name="when">When</param>
        /// <returns>DateTime</returns>
        private DateTime? OrderByDate(string when)
        {
            DateTime d;
            if (DateTime.TryParse(when, out d)) return d;

            return null;
        }

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOid">WorkflowOid</param>
        /// <param name="traces">Traces</param>
        /// <returns>List of WorkflowTraceInfo</returns>
        private IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid workflowOid, List<WorkflowTraceInfo> traces)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var wfdChilds = uofw.WorkflowDefinitions.Find(w => w.WorkflowParentDefinition.WorkflowOid == workflowOid).ToList();
                foreach (var child in wfdChilds)
                {
                    GetTraceForWorkflow(child.WorkflowOid, traces);
                }

                var listTraces = uofw.WorkflowTraces.Find(te => te.WorkflowDefinition.WorkflowOid == workflowOid, te => te.TraceEvent)
                    .Select(te => new
                    {
                        WorkflowOid = workflowOid,
                        te.Message,
                        te.TraceEvent.Type,
                        te.User,
                        te.When,
                        te.Action,
                        te.Code,
                        te.Result
                    }).ToList();

                traces.AddRange(
                    listTraces.Select(te => new WorkflowTraceInfo
                    {
                        WorkflowOid = workflowOid,
                        Message = te.Message,
                        Type = te.Type,
                        User = te.User,
                        When = te.When.ToString(CultureInfo.InvariantCulture),
                        Action = te.Action,
                        Code = te.Code,
                        Result = te.Result
                    }));

                return traces;
            }
        }
        #endregion
    }
}
