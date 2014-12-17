using System.ServiceModel;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Contract
{
    /// <summary>
    /// FlowTasksOperations Interface
    /// <remarks>
    /// Supports until 5 different parallel calls
    /// </remarks>
    /// </summary>
    [ServiceContract(Namespace = "http://flowtasks.com/")]
    public interface IFlowTasksOperations
    {
        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow1", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow1Response")]
        [return: MessageParameter(Name = "response")]
        StartWorkflowResponse StartWorkflow1(StartWorkflowRequest request);

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow2", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow2Response")]
        [return: MessageParameter(Name = "response")]
        StartWorkflowResponse StartWorkflow2(StartWorkflowRequest request);

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow3", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow3Response")]
        [return: MessageParameter(Name = "response")]
        StartWorkflowResponse StartWorkflow3(StartWorkflowRequest request);

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow4", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow4Response")]
        [return: MessageParameter(Name = "response")]
        StartWorkflowResponse StartWorkflow4(StartWorkflowRequest request);

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow5", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/StartWorkflow5Response")]
        [return: MessageParameter(Name = "response")]
        StartWorkflowResponse StartWorkflow5(StartWorkflowRequest request);

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/ApproveTask1", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/ApproveTask1Response")]
        [return: MessageParameter(Name = "response")]
        ApproveTaskResponse ApproveTask1(ApproveTaskRequest request);

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/ApproveTask2", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/ApproveTask2Response")]
        [return: MessageParameter(Name = "response")]
        ApproveTaskResponse ApproveTask2(ApproveTaskRequest request);

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/ApproveTask3", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/ApproveTask3Response")]
        [return: MessageParameter(Name = "response")]
        ApproveTaskResponse ApproveTask3(ApproveTaskRequest request);

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/ApproveTask4", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/ApproveTask4Response")]
        [return: MessageParameter(Name = "response")]
        ApproveTaskResponse ApproveTask4(ApproveTaskRequest request);

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/ApproveTask5", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/ApproveTask5Response")]
        [return: MessageParameter(Name = "response")]
        ApproveTaskResponse ApproveTask5(ApproveTaskRequest request);

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent1", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent1Response")]
        [return: MessageParameter(Name = "response")]
        WorkflowEventResponse WorkflowEvent1(WorkflowEventRequest request);

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent2", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent2Response")]
        [return: MessageParameter(Name = "response")]
        WorkflowEventResponse WorkflowEvent2(WorkflowEventRequest request);

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent3", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent3Response")]
        [return: MessageParameter(Name = "response")]
        WorkflowEventResponse WorkflowEvent3(WorkflowEventRequest request);

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent4", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent4Response")]
        [return: MessageParameter(Name = "response")]
        WorkflowEventResponse WorkflowEvent4(WorkflowEventRequest request);

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent5", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/WorkflowEvent5Response")]
        [return: MessageParameter(Name = "response")]
        WorkflowEventResponse WorkflowEvent5(WorkflowEventRequest request);

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow1", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow1Response")]
        [return: MessageParameter(Name = "response")]
        TerminateWorkflowResponse TerminateWorkflow1(TerminateWorkflowRequest request);

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow2", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow2Response")]
        [return: MessageParameter(Name = "response")]
        TerminateWorkflowResponse TerminateWorkflow2(TerminateWorkflowRequest request);

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow3", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow3Response")]
        [return: MessageParameter(Name = "response")]
        TerminateWorkflowResponse TerminateWorkflow3(TerminateWorkflowRequest request);

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow4", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow4Response")]
        [return: MessageParameter(Name = "response")]
        TerminateWorkflowResponse TerminateWorkflow4(TerminateWorkflowRequest request);

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow5", ReplyAction = "http://flowtasks.com/IFlowTasksOperations/TerminateWorkflow5Response")]
        [return: MessageParameter(Name = "response")]
        TerminateWorkflowResponse TerminateWorkflow5(TerminateWorkflowRequest request);
    }
}
