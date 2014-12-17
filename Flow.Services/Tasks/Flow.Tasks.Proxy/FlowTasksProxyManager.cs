using System;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract;
using System.ServiceModel;

namespace Flow.Tasks.Proxy
{
    /// <summary>
    /// FlowTasksProxyManager
    /// </summary>
    public class FlowTasksProxyManager : IFlowTasksProxyManager
    {
        private IWorkflow _workflow;

        public FlowTasksProxyManager(IWorkflow workflow)
        {
            _workflow = workflow;
        }

        #region IFlowTasksProxyManager Members

        /// <summary>
        /// Get Proxy For Workflow
        /// </summary>
        /// <param name="codeOrId">CodeOrId</param>
        /// <returns>IFlowTasksOperations</returns>
        public IFlowTasksOperations GetProxyForWorkflow(string codeOrId)
        {
            var workflowConfiguration = _workflow.GetWorkflowConfiguration(codeOrId, DateTime.Now);

            FlowTasksOperations proxy;

            if (!string.IsNullOrWhiteSpace(workflowConfiguration.ServiceUrl))
            {
                var config = string.IsNullOrWhiteSpace(workflowConfiguration.BindingConfiguration) ? new BasicHttpBinding() : new BasicHttpBinding(workflowConfiguration.BindingConfiguration);
                proxy = new FlowTasksOperations(config, new EndpointAddress(workflowConfiguration.ServiceUrl));
            }
            else
            {
                proxy = new FlowTasksOperations(workflowConfiguration.ServiceEndPoint);
            }

            return proxy;
            
        }

        #endregion
    }
}
