using System;
using System.Web.Mvc;
using Flow.Tasks.View;
using Flow.Tasks.Contract;
using Flow.Tasks.View.Models;
using Flow.Tasks.Contract.Message;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;

namespace HrView.HrInterview.Controllers
{
    public class HrInterviewController : BaseController
    {
        public HrInterviewController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument document) :
            base(usersService, tasksService, document) { }

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
    }
}
