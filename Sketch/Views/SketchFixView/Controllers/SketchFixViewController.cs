using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Flow.Tasks.Contract;
using Flow.Tasks.View.Models;
using Flow.Tasks.Contract.Message;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;
using Flow.Tasks.View;
using Flow.Tasks.View.Configuration;
using System.Linq;

namespace Sketch.Views.SketchFixView.Controllers
{
    public class SketchFixViewController : BaseController
    {
        private static string DEPLOY = "Deploy";
        private static string ABORT = "Abort";

        public SketchFixViewController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
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
        public ActionResult CompleteFixTask(string completeTask, FormCollection values, TaskModel task)
        {
            // Custom computation
            
            var sketches = TasksService.GetSketchForFilter(new GetSketchForFilterRequest
            {
                Name = values["SketchWorkflowCode"],
                Statuses = new[] { SketchStatusType.DeployedDev, SketchStatusType.SentToSketch }
            });

            var newOid = string.Empty;
            if (sketches.Sketches.Count() > 0)
            {
                newOid = sketches.Sketches.First().XamlxOid.ToString();
            }

            if (completeTask == DEPLOY)
            {
                // Update docsDocument oid with the new one
                TasksService.UpdateWorkflowParameters(new UpdateWorkflowParametersRequest
                {
                    WorkflowOid = values["WorkflowOid"],
                    WfRuntimeValues = new[] {new WfProperty
                        {
                            Name = ConfigHelper.WorkflowPropertyDocumentOid,
                            Type = PropertyType.FlowDoc.ToString(),
                            Value = newOid
                        }
                    }
                });
            }
            if (completeTask == ABORT)
            {
                TasksService.SketchWorkflow(new SketchWorkflowRequest
                {
                    Name = values["SketchWorkflowCode"],
                    ChangedBy = User.Identity.Name,
                    Status = SketchStatusType.Aborted
                });
            }

            // Exit from this area and go back to main control
            return RedirectFromArea(completeTask, values);
        }
    }
}
