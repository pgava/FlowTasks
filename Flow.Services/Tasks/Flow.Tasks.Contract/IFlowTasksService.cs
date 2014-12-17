using System.ServiceModel;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Contract
{
    /// <summary>
    /// FlowTasksService Interface
    /// </summary>
    [ServiceContract(Namespace = "http://flowtasks.com/")]
    public interface IFlowTasksService
    {
        /// <summary>
        /// StartWorkflow creates a new workflow.
        /// </summary>
        /// <example> 
        /// This sample shows how to call the <see cref="StartWorkflow"/> method.   
        /// <code>
        /// using (FlowTasksService proxy = new FlowTasksService())
        /// {
        ///     var startWorkflowResponse = proxy.StartWorkflow(startWorkflowRequest);
        /// }
        /// </code>
        /// </example> 
        /// <param name="request">Specifies the workflow to start.</param>
        /// <returns>On success it returns the workflow ID.</returns>
        /// <remarks>
        /// For more information on the request and response view <see cref="StartWorkflowRequest"/>, <see cref="StartWorkflowResponse"/>
        /// </remarks>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/StartWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/StartWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        StartWorkflowResponse StartWorkflow(StartWorkflowRequest request);

        /// <summary>
        /// Add Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>AddWorkflowResponse</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/AddWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/AddWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        AddWorkflowResponse AddWorkflow(AddWorkflowRequest request);

        /// <summary>
        /// Update Workflow Parameters
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/UpdateWorkflowParameters")]
        void UpdateWorkflowParameters(UpdateWorkflowParametersRequest request);

        /// <summary>
        /// Sketch Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>SketchWorkflowResponse</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/SketchWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/SketchWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        SketchWorkflowResponse SketchWorkflow(SketchWorkflowRequest request);

        /// <summary>
        /// Get Sketch For Filter
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetSketchForFilterResponse</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetSketchForFilter", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetSketchForFilterResponse")]
        [return: MessageParameter(Name = "response")]
        GetSketchForFilterResponse GetSketchForFilter(GetSketchForFilterRequest request);

        /// <summary>
        /// Complete Workflow
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CompleteWorkflow")]
        void CompleteWorkflow(CompleteWorkflowRequest request);

        /// <summary>
        /// Create Workflow
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CreateWorkflow")]
        void CreateWorkflow(CreateWorkflowRequest request);

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CreateTask")]
        void CreateTask(CreateTaskRequest request);

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CreateNotification")]
        void CreateNotification(CreateNotificationRequest request);

        /// <summary>
        /// Complete Task
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CompleteTask")]
        void CompleteTask(CompleteTaskRequest request);

        /// <summary>
        /// Get Expiry TimeSpan
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetExpiryTimeSpan", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetExpiryTimeSpanResponse")]
        [return: MessageParameter(Name = "response")]
        GetExpiryTimeSpanResponse GetExpiryTimeSpan(GetExpiryTimeSpanRequest request);

        /// <summary>
        /// Get Expiry TimeSpan
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetWorkflowResult", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetWorkflowResultResponse")]
        [return: MessageParameter(Name = "response")]
        GetWorkflowResultResponse GetWorkflowResult(GetWorkflowResultRequest request);

        /// <summary>
        /// Is Workflow In Progress
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/IsWorkflowInProgress", ReplyAction = "http://flowtasks.com/IFlowTasksService/IsWorkflowInProgressResponse")]
        [return: MessageParameter(Name = "response")]
        IsWorkflowInProgressResponse IsWorkflowInProgress(IsWorkflowInProgressRequest request);

        /// <summary>
        /// ApproveTask completes the task that was assigned to a user.
        /// </summary>
        /// <example> 
        /// This sample shows how to call the <see cref="ApproveTask"/> method.   
        /// <code>
        /// using (FlowTasksService proxy = new FlowTasksService())
        /// {
        ///     var approveTaskResponse = proxy.ApproveTask(approveTaskRequest);
        /// }
        /// </code>
        /// </example> 
        /// <param name="request">Specifies the task to complete.</param>
        /// <returns>On success it returns the workflow and Task ID.</returns>
        /// <remarks>
        /// For more information on the request and response view <see cref="ApproveTaskRequest"/>, <see cref="ApproveTaskResponse"/>
        /// </remarks>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ApproveTask", ReplyAction = "http://flowtasks.com/IFlowTasksService/ApproveTaskResponse")]
        [return: MessageParameter(Name = "response")]
        ApproveTaskResponse ApproveTask(ApproveTaskRequest request);

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/WorkflowEvent", ReplyAction = "http://flowtasks.com/IFlowTasksService/WorkflowEventResponse")]
        [return: MessageParameter(Name = "response")]
        WorkflowEventResponse WorkflowEvent(WorkflowEventRequest request);

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetNextTasksForUser", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetNextTasksForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetNextTasksForUserResponse GetNextTasksForUser(GetNextTasksForUserRequest request);

        /// <summary>
        /// Get Next Tasks For Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetNextTasksForWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetNextTasksForWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        GetNextTasksForWorkflowResponse GetNextTasksForWorkflow(GetNextTasksForWorkflowRequest request);

        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetTask", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetTaskResponse")]
        [return: MessageParameter(Name = "response")]
        GetTaskResponse GetTask(GetTaskRequest request);

        /// <summary>
        /// Give Back Task
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GiveBackTask")]
        void GiveBackTask(GiveBackTaskRequest request);

        /// <summary>
        /// Hand Over Task To
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/HandOverTaskTo")]
        void HandOverTaskTo(HandOverTaskToRequest request);

        /// <summary>
        /// Get Users For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetUsersForTask", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetUsersForTaskResponse")]
        [return: MessageParameter(Name = "response")]
        GetUsersForTaskResponse GetUsersForTask(GetUsersForTaskRequest request);

        /// <summary>
        /// Get Hand Over Users For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetHandOverUsersForTask", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetHandOverUsersForTaskResponse")]
        [return: MessageParameter(Name = "response")]
        GetHandOverUsersForTaskResponse GetHandOverUsersForTask(GetHandOverUsersForTaskRequest request);

        /// <summary>
        /// Assign Task To
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/AssignTaskTo")]
        void AssignTaskTo(AssignTaskToRequest request);

        /// <summary>
        /// Get Expiry Datetime
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetExpiryDatetime", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetExpiryDatetimeResponse")]
        [return: MessageParameter(Name = "response")]
        GetExpiryDatetimeResponse GetExpiryDatetime(GetExpiryDatetimeRequest request);

        /// <summary>
        /// Get Properties For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetPropertiesForTask", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetPropertiesForTaskResponse")]
        [return: MessageParameter(Name = "response")]
        GetPropertiesForTaskResponse GetPropertiesForTask(GetPropertiesForTaskRequest request);

        /// <summary>
        /// Get Workflow Parameters
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetWorkflowParameters", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetWorkflowParametersResponse")]
        [return: MessageParameter(Name = "response")]
        GetWorkflowParametersResponse GetWorkflowParameters(GetWorkflowParametersRequest request);

        /// <summary>
        /// Search For Tasks
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/SearchForTasks", ReplyAction = "http://flowtasks.com/IFlowTasksService/SearchForTasksResponse")]
        [return: MessageParameter(Name = "response")]
        SearchForTasksResponse SearchForTasks(SearchForTasksRequest request);

        /// <summary>
        /// Get Stats For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetStatsForUser", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetStatsForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetStatsForUserResponse GetStatsForUser(GetStatsForUserRequest request);

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetTraceForWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetTraceForWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        GetTraceForWorkflowResponse GetTraceForWorkflow(GetTraceForWorkflowRequest request);

        /// <summary>
        /// Add Trace To Workflow
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/AddTraceToWorkflow")]
        void AddTraceToWorkflow(AddTraceToWorkflowRequest request);

        /// <summary>
        /// Get Workflow Children
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetWorkflowChildren", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetWorkflowChildrenResponse")]
        [return: MessageParameter(Name = "response")]
        GetWorkflowChildrenResponse GetWorkflowChildren(GetWorkflowChildrenRequest request);

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetWorkflowType", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetWorkflowTypeResponse")]
        [return: MessageParameter(Name = "response")]
        GetWorkflowTypeResponse GetWorkflowType(GetWorkflowTypeRequest request);

        /// <summary>
        /// Get Workflows
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/SearchWorkflows", ReplyAction = "http://flowtasks.com/IFlowTasksService/SearchWorkflowsResponse")]
        [return: MessageParameter(Name = "response")]
        SearchWorkflowsResponse SearchWorkflows(SearchWorkflowsRequest request);

        /// <summary>
        /// Get Workflows
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetWorkflows", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        GetWorkflowResponse GetWorkflows(GetWorkflowRequest request);

        /// <summary>
        /// Cancel Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CancelWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/ControlWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        ControlWorkflowResponse CancelWorkflow(ControlWorkflowRequest request);

        /// <summary>
        /// ReStart Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ReStartWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/ReStartWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        ReStartWorkflowResponse ReStartWorkflow(ReStartWorkflowRequest request);

        /// <summary>
        /// Delete Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/DeleteWorkflow", ReplyAction = "http://flowtasks.com/IFlowTasksService/ControlWorkflowResponse")]
        [return: MessageParameter(Name = "response")]
        ControlWorkflowResponse DeleteWorkflow(ControlWorkflowRequest request);

        /// <summary>
        /// Report User Tasks
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ReportUserTasks", ReplyAction = "http://flowtasks.com/IFlowTasksService/ReportUserTasksResponse")]
        [return: MessageParameter(Name = "response")]
        ReportUserTasksResponse ReportUserTasks(ReportUserTasksRequest request);

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ReportTaskTime", ReplyAction = "http://flowtasks.com/IFlowTasksService/ReportTaskTimeResponse")]
        [return: MessageParameter(Name = "response")]
        ReportTaskTimeResponse ReportTaskTime(ReportTaskTimeRequest request);

        /// <summary>
        /// Report Workflow Time
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ReportWorkflowTime", ReplyAction = "http://flowtasks.com/IFlowTasksService/ReportWorkflowTimeResponse")]
        [return: MessageParameter(Name = "response")]
        ReportWorkflowTimeResponse ReportWorkflowTime(ReportWorkflowTimeRequest request);

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ReportUserTaskCount", ReplyAction = "http://flowtasks.com/IFlowTasksService/ReportUserTaskCountResponse")]
        [return: MessageParameter(Name = "response")]
        ReportUserTaskCountResponse ReportUserTaskCount(ReportUserTaskCountRequest request);

        /// <summary>
        /// Create Topic
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CreateTopic", ReplyAction = "http://flowtasks.com/IFlowTasksService/CreateTopicResponse")]
        [return: MessageParameter(Name = "response")]
        CreateTopicResponse CreateTopic(CreateTopicRequest request);

        /// <summary>
        /// Create Reply
        /// </summary>
        /// <param name="request">Request</param>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/CreateReply")]
        void CreateReply(CreateReplyRequest request);

        /// <summary>
        /// Get Topics For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetTopicsForUser", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetTopicsForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetTopicsForUserResponse GetTopicsForUser(GetTopicsForUserRequest request);

        /// <summary>
        /// Get Replies For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetRepliesForUser", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetRepliesForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetRepliesForUserResponse GetRepliesForUser(GetRepliesForUserRequest request);

        /// <summary>
        /// Get Task Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetTaskCount", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetTaskCountResponse")]
        [return: MessageParameter(Name = "response")]
        GetTaskCountResponse GetTaskCount(GetTaskCountRequest request);

        /// <summary>
        /// Get Topic Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetTopicCount", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetTopicCountResponse")]
        [return: MessageParameter(Name = "response")]
        GetTopicCountResponse GetTopicCount(GetTopicCountRequest request);

        /// <summary>
        /// Search For Topics
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/SearchForTopics", ReplyAction = "http://flowtasks.com/IFlowTasksService/SearchForTopicsResponse")]
        [return: MessageParameter(Name = "response")]
        SearchForTopicsResponse SearchForTopics(SearchForTopicsRequest request);

        /// <summary>
        /// Apply Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/ApplyHoliday", ReplyAction = "http://flowtasks.com/IFlowTasksService/ApplyHolidayResponse")]
        [return: MessageParameter(Name = "response")]
        ApplyHolidayResponse ApplyHoliday(ApplyHolidayRequest request);

        /// <summary>
        /// Apply Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/GetHolidayForUser", ReplyAction = "http://flowtasks.com/IFlowTasksService/GetHolidayForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetHolidayForUserResponse GetHolidayForUser(GetHolidayForUserRequest request);

        /// <summary>
        /// Update Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksService/UpdateHoliday")]
        void UpdateHoliday(UpdateHolidayRequest request);

        /// <summary>
        /// Remove Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        RemoveHolidayResponse RemoveHoliday(RemoveHolidayRequest request);

    }

}
