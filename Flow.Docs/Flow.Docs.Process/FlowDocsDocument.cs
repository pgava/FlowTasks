using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Flow.Library;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Contract;
using Flow.Docs.Contract.Message;
using Flow.Docs.Proxy;

namespace Flow.Docs.Process
{
    /// <summary>
    /// FlowDocsDocument
    /// </summary>
    public class FlowDocsDocument : IFlowDocsDocument
    {
        private const int MaxChunckSize = 131072; //128k

        private readonly IFlowDocsOperations _flowDocsService;

        public FlowDocsDocument(IFlowDocsOperations flowDocsService)
        {
            _flowDocsService = flowDocsService;
        }

        public FlowDocsDocument()
        {

        }

        /// <summary>
        /// Set/Get the size of a chunk
        /// </summary>
        public int ChunkSize { get; set; }

        /// <summary>
        /// Download Document
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="path">Path</param>
        /// <param name="mode">Mode</param>
        public void DownloadDocument(DocumentInfo document, string path, DocumentDownloadMode mode)
        {
            var request = new DownloadDocumentRequest
            {
                OidDocument = document.OidDocument,
                Version = document.Version,
                ChunkNumber = 0,
                ChunkSize = GetChunkSize(),
                Mode = mode
            };

            // Get the first chunk of data
            DownloadDocumentResponse response = null;
            if (_flowDocsService == null)
            {
                using (var docsProxy = new FlowDocsOperations())
                {
                    response = docsProxy.DownloadDocument(request);
                }
            }
            else
            {
                response = _flowDocsService.DownloadDocument(request);
            }

            var filePath = Path.Combine(path, response.DocumentName);
            document.DocumentName = filePath;
            document.Description = response.Description;
            document.Owner = response.Owner;
            document.Path = response.Path;
            document.MimeType = response.MimeType;
            document.FileHash = response.FileHash;

            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                int chunks = 0;
                long curSize = response.ChunkSize;
                writer.Write(response.DataField, 0, (int)Math.Min(response.ChunkSize, response.FileSize));
                request.ChunkNumber = ++chunks;

                // Get all the chunks of the document
                while (chunks < response.ChunkTotal)
                {
                    if (_flowDocsService == null)
                    {
                        using (var docsProxy = new FlowDocsOperations())
                        {
                            response = docsProxy.DownloadDocument(request);
                        }
                    }
                    else
                    {
                        response = _flowDocsService.DownloadDocument(request);
                    }

                    int size = response.ChunkSize;
                    if (response.FileSize > 0)
                    {
                        long bytesLeft = response.FileSize - curSize;
                        size = bytesLeft < response.ChunkSize ? (int)bytesLeft : response.ChunkSize;
                    }
                    
                    writer.Write(response.DataField, 0, size);

                    var hash = Md5Hash.CreateMd5Hash(response.DataField);
                    
                    if (hash != response.FileHash)
                    {
                        throw new Exception(Flow.Library.Properties.Resources.ER_HASH);
                    }

                    curSize += size;
                    request.ChunkNumber = ++chunks;
                }
            }            
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="reader">Reader</param>
        /// <param name="mode">Mode</param>
        /// <returns>Guid</returns>
        private Guid Upload(DocumentInfo document, Stream reader, DocumentUploadMode mode)
        {
            var chunkTotal = (int)Math.Ceiling((decimal)reader.Length / GetChunkSize());
            var chunkSize = GetChunkSize();
            var counter = 0;
            Guid oid = Guid.Empty;
            foreach (byte[] c in SplitIntoChunks(reader, chunkSize))
            {
                var request = new UploadDocumentRequest
                {
                    OidDocument = oid,
                    DocumentName = document.DocumentName,
                    Description = document.Description,
                    Owner = document.Owner,
                    Path = document.Path,
                    Version = document.Version,
                    DataField = c,
                    ChunkNumber = counter++,
                    ChunkTotal = chunkTotal,
                    ChunkSize = chunkSize,
                    MimeType = document.MimeType,
                    FileHash = Md5Hash.CreateMd5Hash(c),
                    FileSize = reader.Length,
                    Mode = mode,
                    PreviousOid = document.PreviousOid
                };

                UploadDocumentResponse response = null;
                if (_flowDocsService == null)
                {
                    using (var docsProxy = new FlowDocsOperations())
                    {
                        response = docsProxy.UploadDocument(request);
                    }
                }
                else
                {
                    response = _flowDocsService.UploadDocument(request);
                }

                oid = response.OidDocument;
            }

            return oid;
        }

#if TEST_ONLY
        public Guid UploadDocument(DocumentInfo document, string content)
        {
            /*
             * NOTE: this is for test only. It has performance issues converting big files
             * into an array of bytes
             */
            using (var reader = new MemoryStream(Encoding.Unicode.GetBytes(content)))
            {
                return Upload(document, reader);
            }
        }
#endif    

        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="content">Content</param>
        /// <param name="mode">Mode</param>
        /// <returns>Guid</returns>
        public Guid UploadDocument(DocumentInfo document, byte[] content, DocumentUploadMode mode)
        {
            document.MimeType = MimeType.GetMimeType(document.DocumentName);
            document.FileHash = "";

            using (var reader = new MemoryStream(content))
            {
                return Upload(document, reader, mode);
            }
        }

        /// <summary>
        /// Upload a document given a path
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="path">Path</param>
        /// <param name="mode">Mode</param>
        /// <returns>Guid</returns>
        public Guid UploadDocument(DocumentInfo document, string path, DocumentUploadMode mode)
        {
            document.MimeType = MimeType.GetMimeType(document.DocumentName);
            document.FileHash = "";

            using (var reader = new FileStream(path, FileMode.Open))
            {
                return Upload(document, reader, mode);
            }
        }

        /// <summary>
        /// Document Info. Get some basic information (like document name)
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public DocumentInfo[] DocumentInfos(Guid[] oids)
        {
            var request = new DocumentInfoRequest
            {
                OidDocuments = oids
            };

            var response = new DocumentInfoResponse();

            if (_flowDocsService == null)
            {
                using (var docsProxy = new FlowDocsOperations())
                {
                    response = docsProxy.DocumentInfos(request);
                }
            }
            else
            {
                response = _flowDocsService.DocumentInfos(request);
            }

            return response.DocumentInfos;
        }

        /// <summary>
        /// Split Into Chunks
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="chunkSize">ChunkSize</param>
        /// <returns>List of chunks</returns>
        private IEnumerable<byte[]> SplitIntoChunks(Stream reader, int chunkSize)
        {
            var count = chunkSize;
            while (count == chunkSize)
            {
                var buffer = new byte[chunkSize];
                count = reader.Read(buffer, 0, count);
                if (count == 0) break;

                yield return buffer;
            }
        }

        /// <summary>
        /// Get Chunk Size
        /// </summary>
        /// <returns>Size</returns>
        private int GetChunkSize()
        {
            return ChunkSize == 0 ? MaxChunckSize : ChunkSize;
        }

    }
}
