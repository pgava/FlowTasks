using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Flow.Tasks.Contract;
using Flow.Tasks.View.Models;
using Flow.Tasks.Contract.Message;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;

namespace Flow.Tasks.View.ReadNotification.Controllers
{
    /// <summary>
    /// ReadNotificationController
    /// </summary>
    public class ReadNotificationController : BaseController
    {
        public ReadNotificationController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        /// <summary>
        /// Index Get
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult Index(string toid)
        {
            return CreateViewFromTaskOid(toid);
        }

        /// <summary>
        /// Index Post
        /// </summary>
        /// <param name="values">Values</param>
        /// <param name="task">Task</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult Index(FormCollection values, TaskModel task)
        {
            return CreateViewFromForm(values, task);
        }

    }
}
