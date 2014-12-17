using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flow.Library;
using Flow.Library.EF;

namespace Flow.Docs.Data
{
    /// <summary>
    /// FlowDocs Context Adapter 
    /// </summary>
    public class FlowDocsContextAdapter : DbContextAdapter
    {
        /// <summary>
        /// Database Name
        /// </summary>
        public override string DatabaseName
        {
            get { return "FlowDocs"; }
        }

        public FlowDocsContextAdapter(FlowDocsEntities flowDocsEntities) : base(flowDocsEntities) { }

    }
}
