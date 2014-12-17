using System.Linq;
using System.IO;
using System;
using Hitone.Web.SqlServerUploader;
using Brettle.Web.NeatUpload;
using Flow.Docs.Data.Core.Interfaces;
using Flow.Docs.Contract.Message;
using Flow.Docs.Data.Core;
using Flow.Docs.Process;
using Flow.Docs.Data.Infrastructure;
using Flow.Library;
using System.Collections.Generic;

/*
 Added method to ~\dotnet\src\Brettle.Web.NeatUpload\Brettle.Web.NeatUpload\UploadStorage.cs
 
 public static UploadStorageProvider ProviderConfig(string path)
 {
    System.Configuration.ConfigXmlDocument e = new System.Configuration.ConfigXmlDocument();
    e.Load(path);

    Config config = Config.CreateFromConfigSection(null, e.FirstChild);
    if (config.DefaultStorageProviderName == null)
    {
        return LastResortProvider;
    }
    return config.StorageProviders[config.DefaultStorageProviderName];
 }
 
 
 */

namespace Flow.Docs.Service
{
    /// <summary>
    /// Implements DocsOperations for database
    /// </summary>
    public sealed class FlowDocsOperationsDatabase : FlowDocsOperationsBase
    {
        /// <summary>
        /// NeatUpload provider
        /// </summary>
        private readonly SqlServerUploadStorageProvider _provider;

        public FlowDocsOperationsDatabase(IFlowDocsUnitOfWork flowDocsUnitOfWork, UploadStorageProvider provider)
            : base(flowDocsUnitOfWork)
        {
            _provider = provider as SqlServerUploadStorageProvider;
        }

        public FlowDocsOperationsDatabase(UploadStorageProvider provider)
        {
            _provider = provider as SqlServerUploadStorageProvider;
        }

        /// <summary>
        /// Upload Chunk
        /// </summary>
        /// <param name="oid">Document Oid</param>
        /// <param name="request">Request</param>
        override protected void UploadChunk(Guid oid, UploadDocumentRequest request)
        {
            if (FlowDocsUnitOfWork != null)
            {
                UploadChunk(FlowDocsUnitOfWork, oid, request);
            }
            else
            {

                using (var uow = new FlowDocsUnitOfWork())
                {
                    UploadChunk(uow, oid, request);
                }
            }
        }

        /// <summary>
        /// Download Chunk
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        override protected void DownloadChunk(DownloadDocumentRequest request, ref DownloadDocumentResponse response)
        {
            if (FlowDocsUnitOfWork != null)
            {
                DownloadChunk(FlowDocsUnitOfWork, request, ref response);
            }
            else
            {
                using (var uow = new FlowDocsUnitOfWork())
                {
                    DownloadChunk(uow, request, ref response);
                }
            }
        }

        /// <summary>
        /// Document Infos
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        override protected void DocumentInfos(DocumentInfoRequest request, ref DocumentInfoResponse response)
        {
            if (FlowDocsUnitOfWork != null)
            {
                DocumentInfos(FlowDocsUnitOfWork, request, ref response);
            }
            else
            {
                using (var uow = new FlowDocsUnitOfWork())
                {
                    DocumentInfos(uow, request, ref response);
                }
            }
        }

        #region Private Methods

        /// <summary>
        /// Download Chunk
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        private void DownloadChunk(IFlowDocsUnitOfWork uow, DownloadDocumentRequest request, ref DownloadDocumentResponse response)
        {
            
            Document doc = null;
            int identity = 0;

            identity = GetDocument(uow, request, ref doc);

            response = new DownloadDocumentResponse();
            response.DocumentName = doc.DocumentName;
            response.Version = doc.Version;
            response.Description = doc.Description;
            response.Owner = doc.Owner;
            response.Path = doc.Path;
            response.MimeType = doc.MimeType;
            response.FileSize = doc.FileSize;

            SqlServerBlobStream blob = null;
            try
            {
                if (_provider.OpenProcedure != null && _provider.ReadProcedure != null)
                {
                    blob = new SqlServerBlobStream(
                        _provider.ConnectionString, identity, _provider.OpenProcedure, _provider.ReadProcedure, FileAccess.Read);
                }
                else
                {
                    blob = new SqlServerBlobStream(
                        _provider.ConnectionString, _provider.TableName, _provider.DataColumnName, identity,
                        _provider.FileNameColumnName, _provider.MIMETypeColumnName, FileAccess.Read);
                }

                blob.Seek(request.ChunkNumber * request.ChunkSize, SeekOrigin.Begin);
                response.DataField = new byte[request.ChunkSize];
                response.ChunkSize = blob.Read(response.DataField, 0, request.ChunkSize);
                response.ChunkNumber = request.ChunkNumber;
                response.ChunkTotal = (int)Math.Ceiling((decimal)blob.Length / request.ChunkSize);
                response.FileHash = Md5Hash.CreateMd5Hash(response.DataField);
            }
            finally
            {
                if (blob != null)
                {
                    blob.Close();
                    //blob.Dispose();
                }
            }
        }

        /// <summary>
        /// Check Hash
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="oid">Oid</param>
        /// <param name="request">Request</param>
        private void CheckHash(IFlowDocsUnitOfWork uow, Guid oid, UploadDocumentRequest request)
        {
            var hash = Md5Hash.CreateMd5Hash(request.DataField);

            if (hash != request.FileHash)
            {
                var attachs = uow.Attachments.Find(a => a.OidDocument == oid);
                foreach (var a in attachs)
                {
                    uow.Attachments.Delete(a);
                }

                uow.Commit();
                throw new Exception(Flow.Library.Properties.Resources.ER_HASH);
            }
        }

        /// <summary>
        /// Process Upload Mode
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="request">Request</param>
        /// <returns>Document</returns>
        private Document ProcessUploadMode(IFlowDocsUnitOfWork uow, UploadDocumentRequest request)
        {
            Document document = null;

            // If don't have the previous oid check by name and path
            if (request.PreviousOid == Guid.Empty)
            {
                var documents = uow.Documents.Find(d => d.DocumentName == request.DocumentName && d.Path == request.Path).OrderByDescending(d => d.Version);
                if (documents.Count() > 0)
                {
                    document = documents.First();
                    request.PreviousOid = document.OidDocument;                        
                }
            }
            else
            {
                document = uow.Documents.FirstOrDefault(d => d.OidDocument == request.PreviousOid);
            }

            if (request.Mode == DocumentUploadMode.Overwrite)
            {                
                if (document != null)
                {
                    uow.Documents.Delete(document);
                }

                document = null;
                request.Version = 1;
            }
            else if (request.Mode == DocumentUploadMode.NewVersion)
            {
                request.Version = document == null ? 1 : document.Version + 1;
            }

            return document;
        }

        /// <summary>
        /// Update Db For Upload
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="oid">Oid</param>
        /// <param name="request">Request</param>
        /// <param name="DbId">DbId</param>
        /// <param name="blobId">blobId</param>
        private void UpdateDbForUpload(IFlowDocsUnitOfWork uow, Guid oid, UploadDocumentRequest request, int DbId, int blobId)
        {
            var newAttachment = uow.Attachments.FirstOrDefault(a => a.AttachmentId == DbId || a.AttachmentId == blobId);
            
            if (request.ChunkNumber == 0)
            {
                if (newAttachment != null)
                {
                    newAttachment.OidDocument = oid;
                    newAttachment.DateCreated = DateTime.Now;
                }
            }

            if (IsLastChunk(request))
            {
                var prevDoc = ProcessUploadMode(uow, request);

                var newDocument = new Document
                {
                    DateCreated = DateTime.Now,
                    DateLastUpdated = DateTime.Now,
                    Description = request.Description,
                    DocumentName = request.DocumentName,
                    OidDocument = oid,
                    Owner = request.Owner,
                    Path = request.Path,
                    Version = request.Version,
                    MimeType = request.MimeType,
                    FileSize = request.FileSize,
                    DocumentPrevious = prevDoc
                };

                uow.Documents.Insert(newDocument);
                if (newAttachment != null) newAttachment.Document = newDocument;
            }

            try
            {
                uow.Commit();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Upload Chunk
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="oid">Oid</param>
        /// <param name="request">Request</param>
        private void UploadChunk(IFlowDocsUnitOfWork uow, Guid oid, UploadDocumentRequest request)
        {
            var dbId = uow.Attachments.FirstOrDefault(a => a.OidDocument == oid);

            int identity = request.ChunkNumber == 0 ? -1 : dbId.AttachmentId;

            CheckHash(uow, oid, request);

            SqlServerBlobStream blob = null;
            try
            {
                blob = new SqlServerBlobStream(
                    _provider.ConnectionString, _provider.TableName, _provider.DataColumnName, _provider.PartialFlagColumnName,
                    _provider.FileNameColumnName, request.DocumentName, _provider.MIMETypeColumnName, "",
                    _provider.CreateProcedure, _provider.OpenProcedure, _provider.WriteProcedure, _provider.ReadProcedure, _provider.CleanupProcedure, _provider.RenameProcedure, _provider.StoreHashProcedure, _provider.DeleteProcedure,
                    identity);

                blob.Seek(0, SeekOrigin.End);
                blob.Write(request.DataField, 0, request.DataField.Length);

            }
            finally
            {
                if (blob != null)
                {
                    blob.Close();
                    //blob.Dispose();
                }
            }

            UpdateDbForUpload(uow, oid, request, identity, blob.Identity);
        }

        /// <summary>
        /// Get Document
        /// </summary>
        /// <param name="uow">Uow</param>
        /// <param name="request">Request</param>
        /// <param name="document">Document</param>
        /// <returns>Document Id</returns>
        private int GetDocument(IFlowDocsUnitOfWork uow, DownloadDocumentRequest request, ref Document document)
        {
            var docs = from d in uow.Documents.AsQueryable()
                       where d.OidDocument == request.OidDocument
                       orderby d.Version descending
                       select d;

            Document doc = docs.FirstOrDefault();
            List<Document> listOfDocs = new List<Document>();

            while (doc != null)
            {
                listOfDocs.Add(doc);
                doc = doc.DocumentPrevious;
            }

            if (request.Mode == DocumentDownloadMode.SpecifiedVersion)
            {
                doc = listOfDocs.Where(d => d.Version == request.Version).FirstOrDefault();
            }
            if (doc == null || request.Mode == DocumentDownloadMode.LastVersion)
            {
                doc = docs.First();
            }

            var dbId = uow.Attachments.First(a => a.DocumentId == doc.DocumentId);

            document = doc;
            return dbId.AttachmentId;
        }

        /// <summary>
        /// Document Infos
        /// </summary>
        /// <param name="uow">FlowDocsUnitOfWork</param>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        private void DocumentInfos(IFlowDocsUnitOfWork uow, DocumentInfoRequest request, ref DocumentInfoResponse response)
        {
            var docs = new List<DocumentInfo>();

            if (request.OidDocuments != null)
            {
                foreach (var doc in request.OidDocuments)
                {
                    var found = uow.Documents.FirstOrDefault(d => d.OidDocument == doc);
                    if (found != null)
                    {
                        docs.Add(new DocumentInfo
                        {
                            DocumentName = found.DocumentName,
                            Description = found.Description,
                            MimeType = found.MimeType,
                            OidDocument = found.OidDocument,
                            Owner = found.Owner
                        });
                    }
                }
            }

            response = new DocumentInfoResponse
            {
                DocumentInfos = docs.ToArray()
            };
        }


        #endregion
    }
}
