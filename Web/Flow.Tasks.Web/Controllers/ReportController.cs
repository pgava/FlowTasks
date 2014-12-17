using System.Globalization;
using System.Web.Mvc;
using System;
using Flow.Tasks.View;
using Flow.Users.Contract;
using Flow.Tasks.Contract;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Web.Controllers
{
  
    /// <summary>
    /// Report Controller
    /// </summary>
    [HandleError]
    [Authorize]
    [FlowTasksAuthorize(UserRoles=new[] {"PM", "MgrPM"})]
    public class ReportController : BaseController
    {
        public ReportController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        /// <summary>
        /// Index action
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Report";

            return View();
        }


        /// <summary>
        /// Report User Tasks
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult ReportUserTasks(string start, string end, string[] users)
        {
            if (ModelState.IsValid)
            {
                var resp = TasksService.ReportUserTasks(new ReportUserTasksRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Users = users
                });

                return Json(resp.Report);
            }
            return Json("Error");
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult ReportTaskTime(string start, string end, string[] tasks, string[] workflows)
        {
            if (ModelState.IsValid)
            {
                var resp = TasksService.ReportTaskTime(new ReportTaskTimeRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Tasks = tasks,
                    Workflows = workflows
                });

                return Json(resp.report);
            }
            return Json("Error");
        }

        /// <summary>
        /// Report Workflow Time
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult ReportWorkflowTime(string start, string end, string[] workflows)
        {
            if (ModelState.IsValid)
            {
                var resp = TasksService.ReportWorkflowTime(new ReportWorkflowTimeRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Workflows = workflows
                });

                return Json(resp.Report);
            }
            return Json("Error");
        }

        public class UserTaskTime
        {
            public string User { get; set; }
            public string Task { get; set; }
            public int Duration { get; set; }
        }

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult ReportUserTaskCount(string start, string end, string[] users, string[] tasks)
        {
            if (ModelState.IsValid)
            {
                var resp = TasksService.ReportUserTaskCount(new ReportUserTaskCountRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Tasks = tasks,
                    Users = users
                });

                return Json(resp.Report);
            }
            return Json("Error");
        }

        /// <summary>
        /// Parse String
        /// </summary>
        /// <param name="d">date to parse</param>
        /// <returns>Datetime</returns>
        private DateTime? ParseString(string d)
        {            
            DateTime date;

            if (DateTime.TryParse(d, out date))
            {
                return date;
            }
            return null;


            if (DateTime.TryParseExact(d, "dd'/'MM'/'yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out date))
            {
                return date;
            }

            return null;
        }
    }
}
