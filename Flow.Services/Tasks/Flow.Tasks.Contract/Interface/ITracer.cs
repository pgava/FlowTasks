using System;
using Flow.Tasks.Contract.Message;
using System.Collections.Generic;

namespace Flow.Tasks.Contract.Interface
{
    /// <summary>
    /// Tracer Interface
    /// </summary>
    public interface ITracer
    {
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
        void Trace(Guid workflowOid, ActionTrace action, string code, string result, string user, string msg, TraceEventType type);

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="action">Action</param>
        /// <param name="code">Code</param>
        /// <param name="msg">Msg</param>
        /// <param name="type">Type</param>
        void Trace(Guid workflowOid, ActionTrace action, string code, string msg, TraceEventType type);

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOids">List of Workflow Oid</param>
        /// <param name="type">Type</param>
        /// <returns>List of Trace</returns>
        IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid[] workflowOids, TraceEventType type);

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOids">List of Workflow Oid</param>
        /// <returns>List of Trace</returns>
        IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid[] workflowOids);

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of Trace</returns>
        IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid workflowOid);

        /// <summary>
        /// Add Trace To Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="taskOid">Task Oid</param>
        /// <param name="user">User</param>
        /// <param name="message">Message</param>
        void AddTraceToWorkflow(Guid workflowOid, Guid taskOid, string user, string message);
    }
}
