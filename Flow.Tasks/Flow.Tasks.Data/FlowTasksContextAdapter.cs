using Flow.Library.EF;

namespace Flow.Tasks.Data
{
    /// <summary>
    /// FlowTasksContext Adapter
    /// </summary>
    public sealed class FlowTasksContextAdapter : DbContextAdapter
    {
        /// <summary>
        /// Database Name
        /// </summary>
        public override string DatabaseName
        {
            get { return "FlowTasks"; }
        }

        public FlowTasksContextAdapter(FlowTasksEntities flowTasksEntities) : base(flowTasksEntities) { }
    }
}
