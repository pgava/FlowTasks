using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Flow.Docs.Contract;
using Flow.Docs.Service;
using Flow.Docs.Contract.Message;
using Ninject;
using Flow.Docs.Process;

namespace Flow.Docs.Service
{
    public class FlowDocsOperations : IFlowDocsOperations
    {
        [Inject]
        public FlowDocsOperationsBase BaseOperations { get; set; }

        #region IFlowDocsOperations Members

        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>UploadDocumentResponse</returns>
        public UploadDocumentResponse UploadDocument(UploadDocumentRequest request)
        {
            return BaseOperations.UploadDocument(request);
        }

        /// <summary>
        /// Download Document
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>DownloadDocumentResponse</returns>
        public DownloadDocumentResponse DownloadDocument(DownloadDocumentRequest request)
        {
            return BaseOperations.DownloadDocument(request);
        }

        /// <summary>
        /// Document Info
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>DocumentInfoResponse</returns>
        public DocumentInfoResponse DocumentInfos(DocumentInfoRequest request)
        {
            return BaseOperations.DocumentInfos(request);
        }

        #endregion
    }
}
