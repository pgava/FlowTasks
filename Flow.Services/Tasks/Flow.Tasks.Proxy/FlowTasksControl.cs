using System;
using Flow.Tasks.Contract;
using System.ServiceModel.Activities;
using System.ServiceModel;

namespace Flow.Tasks.Proxy
{
#if CODE_NOT_USED
    public class FlowTasksControl : IFlowTasksControl
    {
        WorkflowControlClient _controlClient;

        public FlowTasksControl(string clientControlEndPoint)
        {            
            _controlClient = new WorkflowControlClient(new BasicHttpBinding(), new EndpointAddress(clientControlEndPoint));            
        }

        #region IFlowTasksControl Members

        public void CancelWorkflow(Guid workflowOid)
        {
            _controlClient.Cancel(workflowOid);
        }

        #endregion
    }

#endif
}
