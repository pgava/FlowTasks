using Flow.Docs.Contract;
using Flow.Docs.Contract.Message;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Flow.Docs.Proxy
{
    /// <summary>
    /// FlowDocsOperations
    /// </summary>
    public class FlowDocsOperations : ClientBase<IFlowDocsOperations>, IFlowDocsOperations
    {
        #region Ctors
        public FlowDocsOperations()
            : this("FlowDocsOperations_Endpoint")
        {
        }

        public FlowDocsOperations(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public FlowDocsOperations(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowDocsOperations(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowDocsOperations(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        } 
        #endregion

        #region IFlowDocsOperations Members

        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>UploadDocumentResponse</returns>
        public UploadDocumentResponse UploadDocument(UploadDocumentRequest request)
        {
            return Channel.UploadDocument(request);
        }

        /// <summary>
        /// Download Document
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>DownloadDocumentResponse</returns>
        public DownloadDocumentResponse DownloadDocument(DownloadDocumentRequest request)
        {
            return Channel.DownloadDocument(request);
        }

        /// <summary>
        /// Document Info
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public DocumentInfoResponse DocumentInfos(DocumentInfoRequest request)
        {
            return Channel.DocumentInfos(request);
        }

        #endregion
    }
}
