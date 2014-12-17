using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Contract.Message;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Configuration;

namespace Flow.Tasks.View
{
    public class DocUtils
    {
        /// <summary>
        /// FlowDocsDocument
        /// </summary>
        protected IFlowDocsDocument DocsDocument;

        public DocUtils(IFlowDocsDocument docsDocument)
        {
            DocsDocument = docsDocument;
        }

        public IEnumerable<TopicAttachmentInfo> UploadFiles(string user, IEnumerable<HttpPostedFileBase> files, ref string errorMessage)
        {
            var attachments = new List<TopicAttachmentInfo>();

            foreach (var f in files)
            {
                var fileName = string.Empty;
                var oid = UploadFile(user, f, ref fileName, ref errorMessage);
                if (oid != Guid.Empty)
                {
                    attachments.Add(new TopicAttachmentInfo
                    {
                        DocumentOid = oid,
                        FileName = fileName
                    });
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        return attachments;
                }
            }

            return attachments;
        }

        private Guid UploadFile(string user, HttpPostedFileBase fileToUpload, ref string fileName, ref string errorMessage)
        {
            if (fileToUpload == null || fileToUpload.ContentLength <= 0)
                return Guid.Empty;

            if (!CopyFile(fileToUpload, ConfigHelper.DownloadLocation, ref errorMessage))
                return Guid.Empty;

            var fullFileName = GetFullPath(fileToUpload, ConfigHelper.DownloadLocation);

            var newOid = DocsDocument.UploadDocument(new DocumentInfo
            {
                Owner = user,
                DocumentName = Path.GetFileName(fullFileName),
                Description = "File uploaded by user",
                Path = fullFileName
            },
                fullFileName,
                DocumentUploadMode.NewVersion
            );

            fileName = Path.GetFileName(fullFileName);
            return newOid;
        }

        public bool CopyFile(HttpPostedFileBase workflowFile, string serverPath, ref string msg)
        {
            try
            {
                if (workflowFile != null && workflowFile.ContentLength > 0)
                {
                    var filePath = GetFullPath(workflowFile, serverPath);

                    if (File.Exists(filePath))
                    {
                        ArchiveExistingFile(filePath);
                    }
                    workflowFile.SaveAs(filePath);

                    return true;
                }
                msg = "Missing file or file empty";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return false;
        }

        public void ArchiveExistingFile(string filePath)
        {
            var archiveLocation = Path.Combine(ConfigHelper.DownloadLocation, "Archive");
            var currentFolder = Path.Combine(archiveLocation, DateTime.Now.ToString("yyyyMddHHmm"));

            // check for existence of folders
            if (!Directory.Exists(archiveLocation))
            {
                Directory.CreateDirectory(archiveLocation);
            }
            if (!Directory.Exists(currentFolder))
            {
                Directory.CreateDirectory(currentFolder);
            }

            if (string.IsNullOrWhiteSpace(filePath)) return;

            var fname = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fname)) return;

            var newFilePath = Path.Combine(currentFolder, fname);

            File.Move(filePath, newFilePath);
        }

        public string GetFullPath(HttpPostedFileBase workflowFile, string serverPath)
        {
            if (workflowFile != null && workflowFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(workflowFile.FileName);
                Debug.Assert(fileName != null, "fileName != null");
                var filePath = Path.Combine(serverPath, fileName);

                return filePath;
            }

            return string.Empty;
        }

    }

}