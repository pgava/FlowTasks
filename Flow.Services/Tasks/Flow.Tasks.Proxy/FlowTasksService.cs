using System.ServiceModel;
using Flow.Tasks.Contract;
using System.ServiceModel.Channels;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Proxy
{
    /// <summary>
    /// FlowTasksService
    /// </summary>
    public class FlowTasksService : ClientBase<IFlowTasksService>, IFlowTasksService
    {
        #region Ctors
        public FlowTasksService()
            : this("FlowTasksService_Endpoint")
        {
        }

        public FlowTasksService(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public FlowTasksService(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowTasksService(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowTasksService(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        } 
        #endregion

        #region IFlowTasksService Members

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow(StartWorkflowRequest request)
        {
            return Channel.StartWorkflow(request);
        }

        /// <summary>
        /// Add Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>AddWorkflowResponse</returns>
        public AddWorkflowResponse AddWorkflow(AddWorkflowRequest request)
        {
            return Channel.AddWorkflow(request);
        }

        /// <summary>
        /// Update Workflow Parameters
        /// </summary>
        /// <param name="request">Request</param>
        public void UpdateWorkflowParameters(UpdateWorkflowParametersRequest request)
        {
            Channel.UpdateWorkflowParameters(request);
        }

        /// <summary>
        /// Sketch Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>SketchWorkflowResponse</returns>
        public SketchWorkflowResponse SketchWorkflow(SketchWorkflowRequest request)
        {
            return Channel.SketchWorkflow(request);
        }

        /// <summary>
        /// Get Sketch For Filter
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetSketchForFilterResponse</returns>
        public GetSketchForFilterResponse GetSketchForFilter(GetSketchForFilterRequest request)
        {
            return Channel.GetSketchForFilter(request);
        }

        /// <summary>
        /// ApproveTask
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask(ApproveTaskRequest request)
        {
            return Channel.ApproveTask(request);
        }

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent(WorkflowEventRequest request)
        {
            return Channel.WorkflowEvent(request);
        }

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetNextTasksForUserResponse</returns>
        public GetNextTasksForUserResponse GetNextTasksForUser(GetNextTasksForUserRequest request)
        {
            return Channel.GetNextTasksForUser(request);
        }

        /// <summary>
        /// Get Next Tasks For Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetNextTasksForWorkflowResponse</returns>
        public GetNextTasksForWorkflowResponse GetNextTasksForWorkflow(GetNextTasksForWorkflowRequest request)
        {
            return Channel.GetNextTasksForWorkflow(request);
        }

        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetTaskResponse</returns>
        public GetTaskResponse GetTask(GetTaskRequest request)
        {
            return Channel.GetTask(request);
        }

        /// <summary>
        /// Give Back Task
        /// </summary>
        /// <param name="request">Request</param>
        public void GiveBackTask(GiveBackTaskRequest request)
        {
            Channel.GiveBackTask(request);
        }

        /// <summary>
        /// Hand Over Task To
        /// </summary>
        /// <param name="request">Request</param>
        public void HandOverTaskTo(HandOverTaskToRequest request)
        {
            Channel.HandOverTaskTo(request);
        }

        /// <summary>
        /// Get Users For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetUsersForTaskResponse</returns>
        public GetUsersForTaskResponse GetUsersForTask(GetUsersForTaskRequest request)
        {
            return Channel.GetUsersForTask(request);
        }

        /// <summary>
        /// Get Hand Over Users For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetHandOverUsersForTaskResponse</returns>
        public GetHandOverUsersForTaskResponse GetHandOverUsersForTask(GetHandOverUsersForTaskRequest request)
        {
            return Channel.GetHandOverUsersForTask(request);
        }

        /// <summary>
        /// Assign Task To
        /// </summary>
        /// <param name="request">Request</param>
        public void AssignTaskTo(AssignTaskToRequest request)
        {
            Channel.AssignTaskTo(request);
        }

        /// <summary>
        /// Get Expiry Datetime
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetExpiryDatetimeResponse</returns>
        public GetExpiryDatetimeResponse GetExpiryDatetime(GetExpiryDatetimeRequest request)
        {
            return Channel.GetExpiryDatetime(request);
        }

        /// <summary>
        /// Get Properties For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetPropertiesForTaskResponse</returns>
        public GetPropertiesForTaskResponse GetPropertiesForTask(GetPropertiesForTaskRequest request)
        {
            return Channel.GetPropertiesForTask(request);
        }

        /// <summary>
        /// Get Workflow Parameters
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowParametersResponse</returns>
        public GetWorkflowParametersResponse GetWorkflowParameters(GetWorkflowParametersRequest request)
        {
            return Channel.GetWorkflowParameters(request);
        }

        /// <summary>
        /// Search For Tasks
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>SearchForTasksResponse</returns>
        public SearchForTasksResponse SearchForTasks(SearchForTasksRequest request)
        {
            return Channel.SearchForTasks(request);
        }

        /// <summary>
        /// Get Stats For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetStatsForUserResponse</returns>
        public GetStatsForUserResponse GetStatsForUser(GetStatsForUserRequest request)
        {
            return Channel.GetStatsForUser(request);
        }

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetTraceForWorkflowResponse</returns>
        public GetTraceForWorkflowResponse GetTraceForWorkflow(GetTraceForWorkflowRequest request)
        {
            return Channel.GetTraceForWorkflow(request);
        }

        /// <summary>
        /// Add Trace To Workflow
        /// </summary>
        /// <param name="request">Request</param>
        public void AddTraceToWorkflow(AddTraceToWorkflowRequest request)
        {
            Channel.AddTraceToWorkflow(request);
        }

        /// <summary>
        /// Get Workflow Children
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowChildrenResponse</returns>
        public GetWorkflowChildrenResponse GetWorkflowChildren(GetWorkflowChildrenRequest request)
        {
            return Channel.GetWorkflowChildren(request);
        }

        /// <summary>
        /// Cancel Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ControlWorkflowResponse</returns>
        public ControlWorkflowResponse CancelWorkflow(ControlWorkflowRequest request)
        {
            return Channel.CancelWorkflow(request);
        }

        /// <summary>
        /// ReStart Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReStartWorkflowResponse</returns>
        public ReStartWorkflowResponse ReStartWorkflow(ReStartWorkflowRequest request)
        {
            return Channel.ReStartWorkflow(request);
        }

        /// <summary>
        /// Delete Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ControlWorkflowResponse</returns>
        public ControlWorkflowResponse DeleteWorkflow(ControlWorkflowRequest request)
        {
            return Channel.DeleteWorkflow(request);
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateTask(CreateTaskRequest request)
        {
            Channel.CreateTask(request);
        }

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateNotification(CreateNotificationRequest request)
        {
            Channel.CreateNotification(request);
        }

        /// <summary>
        /// Complete Workflow
        /// </summary>
        /// <param name="request">Request</param>
        public void CompleteWorkflow(CompleteWorkflowRequest request)
        {
            Channel.CompleteWorkflow(request);
        }

        /// <summary>
        /// Create Workflow
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateWorkflow(CreateWorkflowRequest request)
        {
            Channel.CreateWorkflow(request);
        }

        /// <summary>
        /// Get Expiry TimeSpan
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowResultResponse</returns>
        public GetWorkflowResultResponse GetWorkflowResult(GetWorkflowResultRequest request)
        {
            return Channel.GetWorkflowResult(request);
        }

        /// <summary>
        /// Is Workflow In Progress
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>IsWorkflowInProgressResponse</returns>
        public IsWorkflowInProgressResponse IsWorkflowInProgress(IsWorkflowInProgressRequest request)
        {
            return Channel.IsWorkflowInProgress(request);
        }

        /// <summary>
        /// Complete Task
        /// </summary>
        /// <param name="request">Request</param>
        public void CompleteTask(CompleteTaskRequest request)
        {
            Channel.CompleteTask(request);
        }

        /// <summary>
        /// Get Expiry TimeSpan
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetExpiryTimeSpanResponse</returns>
        public GetExpiryTimeSpanResponse GetExpiryTimeSpan(GetExpiryTimeSpanRequest request)
        {
            return Channel.GetExpiryTimeSpan(request);
        }

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowTypeResponse</returns>
        public GetWorkflowTypeResponse GetWorkflowType(GetWorkflowTypeRequest request)
        {
            return Channel.GetWorkflowType(request);
        }

        /// <summary>
        /// Search Workflows
        /// </summary>
        /// <param name="request"></param>
        /// <returns>SearchWorkflowsResponse</returns>
        public SearchWorkflowsResponse SearchWorkflows(SearchWorkflowsRequest request)
        {
            return Channel.SearchWorkflows(request);
        }

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowTypeResponse</returns>
        public GetWorkflowResponse GetWorkflows(GetWorkflowRequest request)
        {
            return Channel.GetWorkflows(request);
        }

        /// <summary>
        /// Report User Tasks
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportUserTasksResponse</returns>
        public ReportUserTasksResponse ReportUserTasks(ReportUserTasksRequest request)
        {
            return Channel.ReportUserTasks(request);
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportTaskTimeResponse</returns>
        public ReportTaskTimeResponse ReportTaskTime(ReportTaskTimeRequest request)
        {
            return Channel.ReportTaskTime(request);
        }

        /// <summary>
        /// Report Workflow Time
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportWorkflowTimeResponse</returns>
        public ReportWorkflowTimeResponse ReportWorkflowTime(ReportWorkflowTimeRequest request)
        {
            return Channel.ReportWorkflowTime(request);
        }

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportUserTaskCountResponse</returns>
        public ReportUserTaskCountResponse ReportUserTaskCount(ReportUserTaskCountRequest request)
        {
            return Channel.ReportUserTaskCount(request);
        }

        /// <summary>
        /// Create Topic
        /// </summary>
        /// <param name="request">Request</param>
        public CreateTopicResponse CreateTopic(CreateTopicRequest request)
        {
            return Channel.CreateTopic(request);
        }

        /// <summary>
        /// Create Reply
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateReply(CreateReplyRequest request)
        {
            Channel.CreateReply(request);
        }

        /// <summary>
        /// Get Topics For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetTopicsForUserResponse GetTopicsForUser(GetTopicsForUserRequest request)
        {
            return Channel.GetTopicsForUser(request);
        }

        /// <summary>
        /// Get Replies For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetRepliesForUserResponse GetRepliesForUser(GetRepliesForUserRequest request)
        {
            return Channel.GetRepliesForUser(request);
        }

        /// <summary>
        /// Get Task Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetTaskCountResponse GetTaskCount(GetTaskCountRequest request)
        {
            return Channel.GetTaskCount(request);
        }

        /// <summary>
        /// Get Topic Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetTopicCountResponse GetTopicCount(GetTopicCountRequest request)
        {
            return Channel.GetTopicCount(request);
        }

        /// <summary>
        /// Search For Topics
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public SearchForTopicsResponse SearchForTopics(SearchForTopicsRequest request)
        {
            return Channel.SearchForTopics(request);
        }

        /// <summary>
        /// Apply For Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public ApplyHolidayResponse ApplyHoliday(ApplyHolidayRequest request)
        {
            return Channel.ApplyHoliday(request);
        }

        /// <summary>
        /// Get Holiday For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetHolidayForUserResponse GetHolidayForUser(GetHolidayForUserRequest request)
        {
            return Channel.GetHolidayForUser(request);
        }

        /// <summary>
        /// Update Holiday
        /// </summary>
        /// <param name="request"></param>
        public void UpdateHoliday(UpdateHolidayRequest request)
        {
            Channel.UpdateHoliday(request);
        }

        /// <summary>
        /// Remove Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public RemoveHolidayResponse RemoveHoliday(RemoveHolidayRequest request)
        {
            return Channel.RemoveHoliday(request);
        }

        #endregion
    }
}
