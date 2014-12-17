using System;
using Flow.Docs.Contract.Message;

namespace Flow.Docs.Contract.Interface
{
    /// <summary>
    /// FlowDocs Document Interface
    /// </summary>
    public interface IFlowDocsDocument
    {
        /// <summary>
        /// Chunk Size 
        /// </summary>
        int ChunkSize { get; set; }

        /// <summary>
        /// Download Document
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="path">Path</param>
        /// <param name="mode">Mode</param>
        void DownloadDocument(DocumentInfo document, string path, DocumentDownloadMode mode);

        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="path">Path</param>
        /// <param name="mode">Mode</param>
        /// <returns>Guid</returns>
        Guid UploadDocument(DocumentInfo document, string path, DocumentUploadMode mode);
        
        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="content">Content</param>
        /// <param name="mode">Mode</param>
        /// <returns>Guid</returns>
        Guid UploadDocument(DocumentInfo document, byte[] content, DocumentUploadMode mode);

        /// <summary>
        /// Document Infos
        /// </summary>
        /// <param name="oids">Oids</param>
        /// <returns>List of DocumentInfo</returns>
        DocumentInfo[] DocumentInfos(Guid[] oids);
    }
}
