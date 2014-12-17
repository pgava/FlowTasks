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

namespace Holiday.Views.HolidayUserView.Controllers
{
    public class HolidayUserViewController : BaseController
    {
        private static string Approve = "Approve";
        private static string Reject = "Reject";

        public HolidayUserViewController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
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
        public ActionResult CompleteUserTask(string completeTask, FormCollection values, HttpPostedFileBase workflowFile)
        {
            // Custom computation

            if (completeTask == Approve)
            {
            }
            else if (completeTask == Reject)
            {
            }

            // Exit from this area and go back to main control
            return RedirectFromArea(completeTask, values);
        }
    }
}
