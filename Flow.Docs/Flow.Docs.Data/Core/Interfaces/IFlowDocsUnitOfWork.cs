using System;
using Flow.Library.Interfaces;

namespace Flow.Docs.Data.Core.Interfaces
{
    public interface IFlowDocsUnitOfWork : IUnitOfWork
    {
        IRepository<Document> Documents { get; }
        IRepository<Attachment> Attachments { get; }
    }
}
