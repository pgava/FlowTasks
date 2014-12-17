using Flow.Library.EF;

namespace Flow.Users.Data
{
    /// <summary>
    /// FlowUsers Context Adapter
    /// </summary>
    public sealed class FlowUsersContextAdapter : DbContextAdapter
    {
        /// <summary>
        /// Database Name
        /// </summary>
        public override string DatabaseName
        {
            get { return "FlowUsers"; }
        }

        public FlowUsersContextAdapter(FlowUsersEntities flowUsersEntities) : base(flowUsersEntities) { }
    }
}
