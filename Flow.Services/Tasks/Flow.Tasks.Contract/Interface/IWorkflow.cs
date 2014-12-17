using System;
using System.Collections.Generic;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Contract.Interface
{
    /// <summary>
    /// Workflow Interface
    /// </summary>
    public interface IWorkflow
    {
        /// <summary>
        /// Complete Workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="workflowStatusType">Workflow status</param>
        /// <param name="result">Result</param>
        /// <param name="message">Custom message</param>
        void CompleteWorkflow(Guid workflowOid, WorkflowStatusType workflowStatusType, string result, string message);

        /// <summary>
        /// Create a workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="parentWorkflowOid">Parent workflow oid</param>
        /// <param name="workflowCode">Workflow Code</param>
        /// <param name="domain">Workflow domain</param>
        /// <param name="properties">Workflow list of properties</param>
        void CreateWorkflow(Guid workflowOid, Guid parentWorkflowOid, string workflowCode, string domain, IEnumerable<PropertyInfo> properties);

        /// <summary>
        /// Add Workflow
        /// </summary>
        /// <param name="workflowCode">WorkflowCode</param>
        /// <param name="serviceUrl">ServiceUrl</param>
        /// <param name="bindingConfiguration">BindingConfiguration</param>
        /// <param name="serviceEndpoint">ServiceEndpoint</param>
        void AddWorkflow(string workflowCode, string serviceUrl, string bindingConfiguration, string serviceEndpoint);

        /// <summary>
        /// Update Workflow Parameters
        /// </summary>
        /// <param name="workflowOid">WorkflowOid</param>
        /// <param name="taskOid">TaskOid</param>
        /// <param name="properties">Properties</param>
        void UpdateWorkflowParameters(Guid workflowOid, Guid taskOid, IEnumerable<PropertyInfo> properties);

        /// <summary>
        /// Sketch Workflow
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="xamlxOid">XamlxOid</param>
        /// <param name="changedBy">ChangedBy</param>
        /// <param name="status">Status</param>
        void SketchWorkflow(string name, Guid xamlxOid, string changedBy, SketchStatusType status);

        /// <summary>
        /// Get Sketch For Filter
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="statuses">Statuses</param>
        /// <returns></returns>
        IEnumerable<SketchInfo> GetSketchForFilter(string name, IEnumerable<SketchStatusType> statuses);

        /// <summary>
        /// Get workflow configuration
        /// </summary>
        /// <param name="workflowCode">Workflow Code or Workflow Oid</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <returns>Workflow Configuration</returns>
        WorkflowConfigurationInfo GetWorkflowConfiguration(string workflowCode, DateTime effectiveDate);

        /// <summary>
        /// Get workflow parameters
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of properties</returns>
        IEnumerable<PropertyInfo> GetWorkflowParameters(Guid workflowOid);

        /// <summary>
        /// Get Workflow output parameters
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of properties</returns>
        IEnumerable<PropertyInfo> GetWorkflowOutParameters(Guid workflowOid);

        /// <summary>
        /// Get workflow trace
        /// </summary>
        /// <param name="workflowOids">List of oid</param>
        /// <returns>List of trace</returns>
        IEnumerable<WorkflowTraceInfo> GetTraceForWorkflow(Guid[] workflowOids);

        /// <summary>
        /// Add trace to workflow
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <param name="taskOid">Task Oid</param>
        /// <param name="user">User</param>
        /// <param name="message">Message</param>
        void AddTraceToWorkflow(Guid workflowOid, Guid taskOid, string user, string message);

        /// <summary>
        /// Check workflow is active
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>True if workflow is active</returns>
        bool IsWorkflowInProgress(Guid workflowOid);

        /// <summary>
        /// Delete workflow from the system
        /// </summary>
        /// <param name="workflowOid">Workflow oid</param>
        void DeleteWorkflow(Guid workflowOid);

        /// <summary>
        /// Get workflow children
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of oid</returns>
        IEnumerable<Guid> GetWorkflowChildren(Guid workflowOid);

        /// <summary>
        /// Get the workflow result. Status, Output parameters,...
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>Workflow result</returns>
        WorkflowResultInfo GetWorkflowResult(Guid workflowOid);


        /// <summary>
        /// Search Workflows
        /// </summary>
        /// <param name="workflowCode">WorkflowCode</param>
        /// <param name="domain">Domain</param>
        /// <param name="properties">Properties</param>
        /// <returns></returns>
        IEnumerable<WorkflowInfo> SearchWorkflows(string workflowCode, string domain, IEnumerable<PropertyInfo> properties);

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
        /// <returns>List of WorkflowInfo</returns>
        IEnumerable<WorkflowInfo> GetWorkflows(Guid workflowOid, string workflowCode, string domain, bool isActive, DateTime? startFrom, DateTime? startTo, string user, string role, int pageIndex, int pageSize, out int total, out IEnumerable<string> workflowCodes);

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        /// <returns>List of active WorkflowTypeInfo</returns>
        IEnumerable<WorkflowTypeInfo> GetWorkflowType(DateTime effectiveDate);

        /// <summary>
        /// Report User Tasks
        /// </summary>
        /// <returns>List of User/Tasks</returns>
        IEnumerable<ReportUserTasksInfo> ReportUserTasks(DateTime? start, DateTime? end, IEnumerable<string> users);

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <returns>List of Task/Time</returns>
        IEnumerable<ReportTaskTimeInfo> ReportTaskTime(DateTime? start, DateTime? end, IEnumerable<string> tasks, IEnumerable<string> workflows);

        /// <summary>
        /// Report Workflow Time
        /// </summary>
        /// <returns>List of Workflow/Duration</returns>
        IEnumerable<ReportWorkflowTimeInfo> ReportWorkflowTime(DateTime? start, DateTime? end, IEnumerable<string> workflows);

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <returns>List of User Task/Count</returns>
        IEnumerable<ReportUserTaskCountInfo> ReportUserTaskCount(DateTime? start, DateTime? end, IEnumerable<string> users, IEnumerable<string> tasks);

    }

}
