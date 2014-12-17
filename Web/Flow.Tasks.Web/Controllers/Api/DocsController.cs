using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Contract.Message;
using Flow.Library;
using Flow.Tasks.View;
using Flow.Tasks.View.Configuration;
using Flow.Users.Contract;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class DocsController : BaseApiController
    {
        private DocUtils docUtils;

        public DocsController(IFlowUsersService usersService, IFlowDocsDocument docsDocument)
            : base(usersService, docsDocument)
        {
            docUtils = new DocUtils(docsDocument);
        }

        /// <summary>
        /// Get Document
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/docs/{oid}
        /// </remarks>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetDocument(string oid)
        {
            try
            {
                var info = new DocumentInfo
                {
                    OidDocument = Guid.Parse(oid),
                    Version = 1
                };

                if (!string.IsNullOrWhiteSpace(oid))
                {
                    DocsDocument.DownloadDocument(info, ConfigHelper.DownloadLocation, DocumentDownloadMode.LastVersion);
                }

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(info.DocumentName, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(MimeType.GetMimeType(info.DocumentName));
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(info.DocumentName)
                };

                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

    }
}