using Flow.Library;
using Flow.Library.EF;
using Flow.Library.Interfaces;
using Flow.Docs.Data.Core.Interfaces;
using Flow.Docs.Data.Core;
using System;

namespace Flow.Docs.Data.Infrastructure
{
    /// <summary>
    /// FlowDocs Unit Of Work
    /// </summary>
    public class FlowDocsUnitOfWork : UnitOfWork, IFlowDocsUnitOfWork
    {
        Repository<Document> _documents;
        Repository<Attachment> _attachments;
        readonly IObjectSetFactory _contextAdapter;

        public FlowDocsUnitOfWork(IObjectSetFactory contextAdapter)
            : base(contextAdapter)
        {
            _contextAdapter = contextAdapter;
        }

        public FlowDocsUnitOfWork()
            : base()
        {
            _contextAdapter = new FlowDocsContextAdapter(new FlowDocsEntities());
            ObjectContext = _contextAdapter;
        }

        override protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_contextAdapter != null)
                {
                    _contextAdapter.Dispose();
                }
            }
        }

        #region IFlow.Docs.DataUnitOfWork Members

        public IRepository<Document> Documents
        {
            get { return _documents ?? (_documents = new Repository<Document>(_contextAdapter)); }
        }

        public IRepository<Attachment> Attachments
        {
            get { return _attachments ?? (_attachments = new Repository<Attachment>(_contextAdapter)); }
        }

        #endregion
    }
}
