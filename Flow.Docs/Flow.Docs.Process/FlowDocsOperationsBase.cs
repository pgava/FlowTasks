using System;
using Flow.Docs.Data.Core.Interfaces;
using Flow.Docs.Contract;
using Flow.Docs.Contract.Message;
using System.Collections.Generic;

namespace Flow.Docs.Process
{
    /// <summary>
    /// FlowDocsOperationsBase
    /// </summary>
    public abstract class FlowDocsOperationsBase : IFlowDocsOperations
    {
        /// <summary>
        /// FlowDocs unit of work
        /// </summary>
        protected IFlowDocsUnitOfWork FlowDocsUnitOfWork;

        protected FlowDocsOperationsBase(IFlowDocsUnitOfWork flowDocsUnitOfWork)
        {
            FlowDocsUnitOfWork = flowDocsUnitOfWork;
        }

        protected FlowDocsOperationsBase()
        {
        }

        /// <summary>
        /// Upload Chunk
        /// </summary>
        /// <remarks>
        /// Abstract factory method.
        /// </remarks>
        /// <param name="oid">Oid</param>
        /// <param name="request">UploadDocumentRequest</param>
        abstract protected void UploadChunk(Guid oid, UploadDocumentRequest request);

        /// <summary>
        /// Download Chunk
        /// </summary>
        /// <remarks>
        /// Abstract factory method.
        /// </remarks>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        abstract protected void DownloadChunk(DownloadDocumentRequest request, ref DownloadDocumentResponse response);

        /// <summary>
        /// Document Infos
        /// </summary>
        /// <remarks>
        /// Abstract factory method.
        /// </remarks>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        abstract protected void DocumentInfos(DocumentInfoRequest request, ref DocumentInfoResponse response);

        #region IFlow.Docs.DataService Members

        /// <summary>
        /// Upload Document. It calls the abstract method to perform the operation
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public UploadDocumentResponse UploadDocument(UploadDocumentRequest request)
        {
            if (request.OidDocument != Guid.Empty && request.ChunkNumber == 0)
                throw new ArgumentException("OidDocument should be empty");

            if (request.ChunkNumber > 0 && request.OidDocument == Guid.Empty)
                throw new ArgumentException("OidDocument should not be empty");

            var response = new UploadDocumentResponse
                               {OidDocument = request.OidDocument == Guid.Empty ? Guid.NewGuid() : request.OidDocument};

            UploadChunk(response.OidDocument, request);

            return response;
        }

        /// <summary>
        /// Download Document. It calls the abstract method to perform the operation
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public DownloadDocumentResponse DownloadDocument(DownloadDocumentRequest request)
        {
            if (request.OidDocument == Guid.Empty)
                throw new ArgumentException("OidDocument should not be empty");

            DownloadDocumentResponse response = null;
            DownloadChunk(request, ref response);

            return response;
        }

        /// <summary>
        /// Document Info. Get some basic information (like document name)
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public DocumentInfoResponse DocumentInfos(DocumentInfoRequest request)
        {
            DocumentInfoResponse response = null;
            DocumentInfos(request, ref response);

            return response;
        }

        /// <summary>
        /// Is Last Chunk
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>True or False</returns>
        protected bool IsLastChunk(UploadDocumentRequest request)
        {
            return request.ChunkNumber == request.ChunkTotal - 1;
        }

        #endregion
    }
}
