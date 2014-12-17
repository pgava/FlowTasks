namespace Flow.Tasks.Contract
{
    /// <summary>
    /// FlowTasksProxyManager Interface
    /// </summary>
    public interface IFlowTasksProxyManager
    {
        /// <summary>
        /// Get Proxy For Workflow
        /// </summary>
        /// <param name="codeOrId">Workfow Code or Id</param>
        /// <returns>FlowTasksOperations proxy</returns>
        IFlowTasksOperations GetProxyForWorkflow(string codeOrId);
    }
}
