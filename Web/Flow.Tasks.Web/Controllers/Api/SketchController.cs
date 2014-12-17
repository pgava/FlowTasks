using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml;
using Flow.Docs.Contract.Message;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Configuration;

namespace Flow.Tasks.Web.Controllers.Api
{

    public class SketchController : BaseApiController
    {
        // default this file not get
        private const string FDEFAULT = "rootFlowchart - save file.xamlx";

        public SketchController(IFlowTasksService tasksService)
            : base(tasksService)
        {
        }

        /// <summary>
        /// Get Flow Tasks
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/sketch/op/tasks
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName("tasks")]
        public IEnumerable<string> GetFlowTasks()
        {
            return new List<string> { "Generic", "Approve" };
        }

        /// <summary>
        /// Get Flow Tasks
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/sketch/op/types
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName("types")]
        public IEnumerable<string> GetTypes()
        {
            return new List<string> { "Int32", "Object", "String" };
        }

        /// <summary>
        /// Deploy
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/sketch/op/deploy?fname=wf1
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [ActionName("deploy")]
        public HttpResponseMessage Deploy(string fname)
        {
            try
            {
                //return new HttpResponseMessage(HttpStatusCode.Created);

                var sketches = TasksService.GetSketchForFilter(new GetSketchForFilterRequest
                {
                    Name = fname,
                    Statuses = new[] { SketchStatusType.Saved }
                });

                if (sketches.Sketches.Any())
                {
                    var startWorkflowRequest = new StartWorkflowRequest
                    {
                        Domain = ConfigHelper.WorkflowDomain,
                        WorkflowCode = ConfigHelper.WorkflowCodeSketch,
                        WfRuntimeValues = new[] 
                        {
                            new WfProperty
                            {
                                Name = ConfigHelper.WorkflowPropertyDocumentOid,
                                Type = PropertyType.FlowDoc.ToString(),
                                Value = sketches.Sketches[0].XamlxOid.ToString()
                            },
                            new WfProperty
                            {
                                Name = ConfigHelper.WorkflowPropertyCode,
                                Type = PropertyType.String.ToString(),
                                Value = fname
                            }
                        }
                    };

                    TasksService.StartWorkflow(startWorkflowRequest);
                }

                TasksService.SketchWorkflow(new SketchWorkflowRequest
                {
                    Name = fname,
                    ChangedBy = User.Identity.Name,
                    Status = SketchStatusType.DeployedDev
                });

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/sketch/op/save?fname=wf1
        /// </remarks>
        /// <param name="fname"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("save")]
        public HttpResponseMessage Save(string fname, [FromBody]string content)
        {
            try
            {
                //return new HttpResponseMessage(HttpStatusCode.Created);

                content = content.Replace("\\n", "&#xD;&#xA;").Replace("< ", "<").Replace(" >", ">");

                var newOid = DocsDocument.UploadDocument(new DocumentInfo
                {
                    Owner = User.Identity.Name,
                    DocumentName = fname + ".xamlx",
                    Description = "Sketch Workflow",
                    Path = "/SketchWorkFlows/"
                },
                    Encoding.ASCII.GetBytes(content),
                    DocumentUploadMode.Overwrite
                );

                var sketches = TasksService.GetSketchForFilter(new GetSketchForFilterRequest
                {
                    Name = fname,
                    Statuses = new[] { SketchStatusType.SentToSketch }
                });

                SketchStatusType sketchStatus = sketches.Sketches.Count() > 0 ? SketchStatusType.SentToSketch : SketchStatusType.Saved;

                TasksService.SketchWorkflow(new SketchWorkflowRequest
                {
                    Name = fname,
                    ChangedBy = User.Identity.Name,
                    Status = sketchStatus,
                    XamlxOid = newOid.ToString()
                });

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get Sketch
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/sketch/wf1 or 
        /// http://localhost/Flow.tasks.web/api/sketch/_empty_ to get an empty workflow
        /// </remarks>
        /// <param name="fname"></param>
        /// <returns></returns>
        [HttpGet]
        public string Get(string fname)
        {
            try
            {
                //return "loading: " + fname;

                if (fname.Equals("_empty_"))
                {
                    string path = HostingEnvironment.MapPath("~/WorkFlows/template/" + FDEFAULT);
                    if (File.Exists(path))
                    {
                        XmlDocument form = new XmlDocument();
                        form.Load(path);
                        string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(form);

                        return json;
                    }
                }
                else
                {
                    var sketches = TasksService.GetSketchForFilter(new GetSketchForFilterRequest
                    {
                        Name = fname,
                        Statuses = new[] { SketchStatusType.Saved, SketchStatusType.SentToSketch }
                    });

                    if (sketches.Sketches.Count > 0)
                    {
                        string path = "/SketchWorkFlows/";
                        string fileName = fname + ".xamlx";

                        DocsDocument.DownloadDocument(
                            new DocumentInfo
                            {
                                OidDocument = sketches.Sketches[0].XamlxOid,
                                DocumentName = fileName,
                                Path = path
                            },
                            ConfigHelper.DownloadLocation, DocumentDownloadMode.LastVersion);

                        XmlDocument xamlx = new XmlDocument();
                        xamlx.Load(Path.Combine(ConfigHelper.DownloadLocation, fileName));

                        //convert xml to json
                        string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xamlx);

                        return json;
                    }
                }

                var result = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(result);
            }
            catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                throw new HttpResponseException(result);
            }
        }

    }
}