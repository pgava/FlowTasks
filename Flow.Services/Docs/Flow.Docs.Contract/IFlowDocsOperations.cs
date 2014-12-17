using Flow.Docs.Contract.Message;
using System.ServiceModel;

namespace Flow.Docs.Contract
{
    /// <summary>
    /// FlowDocsOperations Interface
    /// </summary>
    [ServiceContract(Namespace = "http://flowtasks.com/")]
    public interface IFlowDocsOperations
    {
        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowDocsOperations/UploadDocument", ReplyAction = "http://flowtasks.com/IFlowDocsOperations/UploadDocumentResponse")]
        [return: MessageParameter(Name = "response")]
        UploadDocumentResponse UploadDocument(UploadDocumentRequest request);

        /// <summary>
        /// Download Document
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowDocsOperations/DownloadDocument", ReplyAction = "http://flowtasks.com/IFlowDocsOperations/DownloadDocumentResponse")]
        [return: MessageParameter(Name = "response")]
        DownloadDocumentResponse DownloadDocument(DownloadDocumentRequest request);

        /// <summary>
        /// Document Info
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowDocsOperations/DocumentInfo", ReplyAction = "http://flowtasks.com/IFlowDocsOperations/DocumentInfoResponse")]
        [return: MessageParameter(Name = "response")]
        DocumentInfoResponse DocumentInfos(DocumentInfoRequest request);
    }
}
