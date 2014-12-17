using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Flow.Library;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Contract;
using Flow.Docs.Contract.Message;

namespace Flow.Docs.Data.Process
{
    public class FlowDocsDocument : IFlowDocsDocument
    {
        private const int MaxChunckSize = 16384; //1024*16

        private readonly IFlowDocsOperations _flowDocsService;

        public FlowDocsDocument(IFlowDocsOperations flowDocsService)
        {
            _flowDocsService = flowDocsService;
        }

        public int ChunkSize { get; set; }


        public void DownloadDocument(DocumentInfo document, string path)
        {
            var request = new DownloadDocumentRequest
            {
                OidDocument = document.OidDocument,
                Version = document.Version,
                ChunkNumber = 0,
                ChunkSize = GetChunkSize()
            };
            
            DownloadDocumentResponse response = _flowDocsService.DownloadDocument(request);
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
                writer.Write(response.DataField, 0, response.ChunkSize);
                request.ChunkNumber = ++chunks;

                while (chunks < response.ChunkTotal)
                {
                    response = _flowDocsService.DownloadDocument(request);
                    writer.Write(response.DataField, 0, response.ChunkSize);
                    request.ChunkNumber = ++chunks;
                }
            }            
        }

        private Guid Upload(DocumentInfo document, Stream reader)
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
                    MimeType = document.MimeType
                };
                var response = _flowDocsService.UploadDocument(request);
                oid = response.OidDocument;
            }

            return oid;
        }

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

        public void UploadDocument(DocumentInfo document, byte[] content)
        {
            document.MimeType = MimeType.GetMimeType(document.DocumentName);
            document.FileHash = "";

            using (var reader = new MemoryStream(content))
            {
                Upload(document, reader);
            }
        }

        public Guid Upload(DocumentInfo document, string path)
        {
            document.MimeType = MimeType.GetMimeType(document.DocumentName);
            document.FileHash = "";

            using (var reader = new FileStream(path, FileMode.Open))
            {
                return Upload(document, reader);
            }
        }

        public string CreateMd5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
                // To force the hex string to lower-case letters instead of
                // upper-case, use the following line instead:
                // sb.Append(hashBytes[i].ToString("x2")); 
            }
            return sb.ToString();
        }

// ReSharper disable MemberCanBeMadeStatic.Local
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

        private int GetChunkSize()
        {
            return ChunkSize == 0 ? MaxChunckSize : ChunkSize;
        }

    }
}
