using System.ServiceModel;
using Flow.Tasks.Contract;
using System.ServiceModel.Channels;
using Flow.Tasks.Contract.Message;
    
namespace Flow.Tasks.Proxy
{
    /// <summary>
    /// FlowTasksOperations
    /// </summary>
    public class FlowTasksOperations : ClientBase<IFlowTasksOperations>, IFlowTasksOperations
    {
        #region Ctors
        public FlowTasksOperations()
        {
        }

        public FlowTasksOperations(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public FlowTasksOperations(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowTasksOperations(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowTasksOperations(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        #endregion

        #region Start Workflow
        
        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow1(StartWorkflowRequest request)
        {
            return Channel.StartWorkflow1(request);
        }

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow2(StartWorkflowRequest request)
        {
            return Channel.StartWorkflow2(request);
        }

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow3(StartWorkflowRequest request)
        {
            return Channel.StartWorkflow3(request);
        }

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow4(StartWorkflowRequest request)
        {
            return Channel.StartWorkflow4(request);
        }

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow5(StartWorkflowRequest request)
        {
            return Channel.StartWorkflow5(request);
        }        
        #endregion

        #region Approve Task

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask1(ApproveTaskRequest request)
        {
            return Channel.ApproveTask1(request);
        }

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask2(ApproveTaskRequest request)
        {
            return Channel.ApproveTask2(request);
        }

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask3(ApproveTaskRequest request)
        {
            return Channel.ApproveTask3(request);
        }

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask4(ApproveTaskRequest request)
        {
            return Channel.ApproveTask4(request);
        }

        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask5(ApproveTaskRequest request)
        {
            return Channel.ApproveTask5(request);
        } 
        #endregion

        #region Workflow Event
        
        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent1(WorkflowEventRequest request)
        {
            return Channel.WorkflowEvent1(request);
        }

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent2(WorkflowEventRequest request)
        {
            return Channel.WorkflowEvent2(request);
        }

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent3(WorkflowEventRequest request)
        {
            return Channel.WorkflowEvent3(request);
        }

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent4(WorkflowEventRequest request)
        {
            return Channel.WorkflowEvent4(request);
        }

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent5(WorkflowEventRequest request)
        {
            return Channel.WorkflowEvent5(request);
        }
        #endregion

        #region Terminate Workflow

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>TerminateWorkflowResponse</returns>
        public TerminateWorkflowResponse TerminateWorkflow1(TerminateWorkflowRequest request)
        {
            return Channel.TerminateWorkflow1(request);
        }

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>TerminateWorkflowResponse</returns>
        public TerminateWorkflowResponse TerminateWorkflow2(TerminateWorkflowRequest request)
        {
            return Channel.TerminateWorkflow2(request);
        }

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>TerminateWorkflowResponse</returns>
        public TerminateWorkflowResponse TerminateWorkflow3(TerminateWorkflowRequest request)
        {
            return Channel.TerminateWorkflow3(request);
        }

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>TerminateWorkflowResponse</returns>
        public TerminateWorkflowResponse TerminateWorkflow4(TerminateWorkflowRequest request)
        {
            return Channel.TerminateWorkflow4(request);
        }

        /// <summary>
        /// Terminate Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>TerminateWorkflowResponse</returns>
        public TerminateWorkflowResponse TerminateWorkflow5(TerminateWorkflowRequest request)
        {
            return Channel.TerminateWorkflow5(request);
        } 
        #endregion
    }    
}
