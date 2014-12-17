using System.Web.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Flow.Tasks.View;
using Flow.Users.Contract;
using Flow.Tasks.Contract;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Contract.Message;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Configuration;
using System.Text;
using Flow.Tasks.View.Models;
// using system for get file and convert svg to xml

namespace Flow.Tasks.Web.Controllers
{

    [HandleError]
    [Authorize]
    [FlowTasksAuthorize(UserRoles=new[] {"BA", "MgrBA"})]
    public class SketchController : BaseController
    {
        private const string SKETCH_TASK_CODE = "SketchFix";
        private const string SKETCH_WORKFLOW_PROP = "SketchWorkflowCode";

        public SketchController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        public ActionResult Index(string workflow)
        {
            ViewBag.Message = "Sketch";

            var model = new SketchModel
            {
                Workflow = workflow,
                RedirectTask = CreateTaskModelFromDatabase(SKETCH_TASK_CODE, User.Identity.Name, 
                    new List<PropertyInfo>
                    {
                        new PropertyInfo{Name = SKETCH_WORKFLOW_PROP, Value = workflow}
                    })
            };

            return View(model);
        }

        // default this file not get
        private const string FDEFAULT = "rootFlowchart - save file.xamlx";

        [HttpPost] // get workflow tasks
        public ActionResult GetFlowTasks()
        {
            if (ModelState.IsValid)
            {
                var tasks = new List<string> {"Generic", "Approve"};

                return Json(tasks);
            }
            return Json("Error");
        }

        [HttpPost] // get workflow codes
        public ActionResult GetWorkflowCodesFile()
        {
            if (ModelState.IsValid)
            {
                // get directory of files
                string path = Server.MapPath("~/WorkFlows");
                string[] files = Directory.GetFiles(path, "*.xamlx");

                // cut string get file name
                var fnames = new List<string>();
                foreach (string file in files)
                {
                    string[] val = file.Split('\\');
                    string fname = val[val.Length - 1];
                    if (fname != FDEFAULT)
                    {
                        fnames.Add(fname.Split('.')[0]);
                    }
                }
                return Json(fnames);
            }
            return Json("Error");
        }

        [HttpPost] // get workflow codes
        public ActionResult GetWorkflowCodes()
        {
            if (ModelState.IsValid)
            {
                var fnames = new List<string>();

                var sketches = TasksService.GetSketchForFilter(new GetSketchForFilterRequest
                {
                    Statuses = new[] { SketchStatusType.Saved, SketchStatusType.SentToSketch }
                });

                foreach (var s in sketches.Sketches)
                {
                    fnames.Add(s.Name);
                }

                return Json(fnames);
            }
            return Json("Error");
        }

        [HttpPost] // get workflows
        public ActionResult GetWorkflow(string code)
        {
            if (ModelState.IsValid)
            {
                return Json("workflow");
            }
            return Json("Error");
        }

        [HttpPost] // get type variables
        public ActionResult GetTypes()
        {
            if (ModelState.IsValid)
            {
                var types = new List<string> {"Int32", "Object", "String"};

                return Json(types);
            }
            return Json("Error");
        }

        [HttpPost] // load file xamlx in server
        public ActionResult LoadWorkflowFile(string fname)
        {
            if (ModelState.IsValid)
            {
                // load xamlx file 
                string path = Server.MapPath("~/WorkFlows/" + fname + ".xamlx");
                if (System.IO.File.Exists(path))
                {
                    XmlDocument xamlx = new XmlDocument();
                    xamlx.Load(path);

                    //convert xml to json
                    String json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xamlx);

                    return Json(json);
                }
                return Json("Error");
            }
            return Json("Error");
        }

        [HttpPost] // load file xamlx in server
        public ActionResult LoadWorkflow(string fname)
        {
            if (ModelState.IsValid)
            {
                var sketches = TasksService.GetSketchForFilter(new GetSketchForFilterRequest
                {
                    Name = fname,
                    Statuses = new[] {SketchStatusType.Saved, SketchStatusType.SentToSketch}
                });
                
                if (sketches.Sketches.Count > 0)
                {
                    string path = "/SketchWorkFlows/";
                    string fileName = fname + ".xamlx";

                    DocsDocument.DownloadDocument(new DocumentInfo { OidDocument = sketches.Sketches[0].XamlxOid, DocumentName = fileName, Path = path }, 
                        ConfigHelper.DownloadLocation, DocumentDownloadMode.LastVersion);

                    XmlDocument xamlx = new XmlDocument();
                    xamlx.Load(Path.Combine(ConfigHelper.DownloadLocation, fileName));

                    //convert xml to json
                    String json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xamlx);

                    return Json(json);
                }
                return Json("Error");
            }
            return Json("Error");
        }

        [HttpPost] // from flowchart
        public ActionResult GetFormFlowchart()
        {
            if (ModelState.IsValid)
            {
                string path = Server.MapPath("~/WorkFlows/template/" + FDEFAULT);
                if (System.IO.File.Exists(path)) 
                {
                    XmlDocument form = new XmlDocument();
                    form.Load(path);
                    String json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(form);

                    return Json(json);
                }
                return Json("Error");
            }
            return Json("Error");
        }

        [HttpPost] // 
        public ActionResult SaveWorkflowFile(string fname, string content)
        {
            if (ModelState.IsValid)
            {
                string path = Server.MapPath("~/WorkFlows/" + fname + ".xamlx");

                content = content.Replace("\\n", "&#xD;&#xA;").Replace("< ", "<").Replace(" >", ">");
                System.IO.File.WriteAllText(path, content);

                return Json("WorkFlow Saved!");
            }
            return Json("Error");
        }

        [HttpPost] // 
        public ActionResult DeployWorkflow(string fname, string content)
        {
            if (ModelState.IsValid)
            {
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

                return Json("WorkFlow Deployed!");
            }
            return Json("Error");
        }

        [HttpPost] // 
        public ActionResult SaveWorkflow(string fname, string content)
        {
            if (ModelState.IsValid)
            {
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

                return Json("WorkFlow Saved!");
            }
            return Json("Error");
        }
    }
}

