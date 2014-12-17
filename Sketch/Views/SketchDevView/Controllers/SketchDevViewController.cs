using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Flow.Tasks.Contract;
using Flow.Tasks.View.Models;
using Flow.Tasks.Contract.Message;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;
using Flow.Tasks.View;
using System.Web;
using System.IO;
using Flow.Tasks.View.Configuration;
using Flow.Docs.Contract.Message;

namespace Sketch.Views.SketchDevView.Controllers
{
    public class SketchDevViewController : BaseController
    {
        private static string DEPLOY = "Deploy";
        private static string SEND_BACK = "Send Back";

        public SketchDevViewController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        [HttpGet]
        public ActionResult Index(string toid)
        {
            return CreateViewFromTaskOid(toid);
        }

        [HttpPost]
        public ActionResult Index(FormCollection values, TaskModel task)
        {
            return CreateViewFromForm(values, task);
        }

        [HttpPost]
        public ActionResult UploadFile(string completeTask, FormCollection values, HttpPostedFileBase workflowFile)
        {
            // Custom computation

            var oldOid = GetDocumentOid(values["TaskOid"]);
            if (completeTask == DEPLOY)
            {
                string msg = string.Empty;
                CopyFile(workflowFile, values["TaskParameterSketchWorkflowPath"], ref msg);
                var workflowFullFileName = GetFullPath(workflowFile, values["TaskParameterSketchWorkflowPath"]);

                var newOid = DocsDocument.UploadDocument(new DocumentInfo
                    {
                        Owner = User.Identity.Name,
                        DocumentName = Path.GetFileName(workflowFullFileName),
                        Description = "Sketch Workflow",
                        OidDocument = Guid.Parse(oldOid),
                        Path = "/SketchWorkFlows/"
                    },
                    GetFullPath(workflowFile, values["TaskParameterSketchWorkflowPath"]),
                    DocumentUploadMode.Overwrite
                );

                TasksService.AddWorkflow(new AddWorkflowRequest
                {
                    WorkflowCode = values["TaskParameterSketchWorkflowCode"].ToString(),
                    ServiceUrl = values["TaskParameterSketchWorkflowUrl"].ToString() + Path.GetFileName(workflowFile.FileName),
                    BindingConfiguration = "BasicHttpBinding_FlowTasks"
                });

                TasksService.SketchWorkflow(new SketchWorkflowRequest
                {
                    Name = values["TaskParameterSketchWorkflowCode"].ToString(),
                    ChangedBy = User.Identity.Name,
                    Status = SketchStatusType.DeployedProd,
                    XamlxOid = newOid.ToString()
                });
            }
            else if (completeTask == SEND_BACK)
            {
                TasksService.SketchWorkflow(new SketchWorkflowRequest
                {
                    Name = values["TaskParameterSketchWorkflowCode"].ToString(),
                    ChangedBy = User.Identity.Name,
                    Status = SketchStatusType.SentToSketch,
                    XamlxOid = oldOid
                });
            }

            // Exit from this area and go back to main control
            return RedirectFromArea(completeTask, values);
        }
    }
}
