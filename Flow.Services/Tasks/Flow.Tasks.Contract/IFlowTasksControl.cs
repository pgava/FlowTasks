using System;

namespace Flow.Tasks.Contract
{
    /// <summary>
    /// FlowTasksControl Interface
    /// <remarks>
    /// This is now deprecated.
    /// </remarks>
    /// </summary>
    public interface IFlowTasksControl
    {
        void CancelWorkflow(Guid workflowOid);
    }
}
