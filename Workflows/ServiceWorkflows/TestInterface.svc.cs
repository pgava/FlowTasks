using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;

namespace ServiceWorkflows
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TestInterface" in code, svc and config file together.
    public class TestInterface : IFlowTasksOperations
    {

        #region IFlowTasksOperations Members

        public StartWorkflowResponse StartWorkflow1(StartWorkflowRequest request)
        {
            return new StartWorkflowResponse { WorkflowId = "" };
        }

        public StartWorkflowResponse StartWorkflow2(StartWorkflowRequest request)
        {
            return new StartWorkflowResponse { WorkflowId = "" };
        }

        public StartWorkflowResponse StartWorkflow3(StartWorkflowRequest request)
        {
            return new StartWorkflowResponse { WorkflowId = "" };
        }

        public StartWorkflowResponse StartWorkflow4(StartWorkflowRequest request)
        {
            return new StartWorkflowResponse { WorkflowId = "" };
        }

        public StartWorkflowResponse StartWorkflow5(StartWorkflowRequest request)
        {
            return new StartWorkflowResponse { WorkflowId = "" };
        }

        public ApproveTaskResponse ApproveTask1(ApproveTaskRequest request)
        {
            throw new NotImplementedException();
        }

        public ApproveTaskResponse ApproveTask2(ApproveTaskRequest request)
        {
            throw new NotImplementedException();
        }

        public ApproveTaskResponse ApproveTask3(ApproveTaskRequest request)
        {
            throw new NotImplementedException();
        }

        public ApproveTaskResponse ApproveTask4(ApproveTaskRequest request)
        {
            throw new NotImplementedException();
        }

        public ApproveTaskResponse ApproveTask5(ApproveTaskRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkflowEventResponse WorkflowEvent1(WorkflowEventRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkflowEventResponse WorkflowEvent2(WorkflowEventRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkflowEventResponse WorkflowEvent3(WorkflowEventRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkflowEventResponse WorkflowEvent4(WorkflowEventRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkflowEventResponse WorkflowEvent5(WorkflowEventRequest request)
        {
            throw new NotImplementedException();
        }

        public TerminateWorkflowResponse TerminateWorkflow1(TerminateWorkflowRequest request)
        {
            throw new NotImplementedException();
        }

        public TerminateWorkflowResponse TerminateWorkflow2(TerminateWorkflowRequest request)
        {
            throw new NotImplementedException();
        }

        public TerminateWorkflowResponse TerminateWorkflow3(TerminateWorkflowRequest request)
        {
            throw new NotImplementedException();
        }

        public TerminateWorkflowResponse TerminateWorkflow4(TerminateWorkflowRequest request)
        {
            throw new NotImplementedException();
        }

        public TerminateWorkflowResponse TerminateWorkflow5(TerminateWorkflowRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
