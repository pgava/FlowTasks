using System;
using System.Web.Mvc;
using Flow.Tasks.Contract;
using Flow.Tasks.View.Models;
using Flow.Tasks.Contract.Message;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;
using Flow.Tasks.View;
using System.Linq;

namespace Holiday.Views.HolidayMgrView.Controllers
{
    public class HolidayMgrViewController : BaseController
    {
        private static string Approve = "Approve";
        private static string Reject = "Reject";

        public HolidayMgrViewController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        [HttpGet]
        public ActionResult Index(string toid)
        {

            if (!string.IsNullOrWhiteSpace(toid))
            {
                var task = TasksService.GetTask(new GetTaskRequest { TaskOid = Guid.Parse(toid) });
                var docs = GetAttachedDocuments(task.Task.TaskOid);
                var comments = GetCommentsForTask(task.Task.WorkflowOid.ToString());
                var parameters = GetTaskParameters(task.Task.TaskOid);

                var model = CreateTask(task.Task, docs, comments, parameters);
                var idStr = model.GetPropertyValueFromName("HolidayId");
                var name = model.GetPropertyValueFromName("UserName");
                int id;
                int.TryParse(idStr, out id);

                if (id > 0)
                {
                    var res = TasksService.GetHolidayForUser(new GetHolidayForUserRequest { HolidayId = id, User = name });
                    var holiday = res.Holidays.First();
                    if (holiday != null)
                    {
                        var dates = holiday.Holiday.Select(DateTime.Parse).OrderBy(d => d).ToList();
                        var datesToString = dates.Select(d => d.ToString("ddd d MMM"));
                        ViewBag.Dates = datesToString.ToArray();
                    }
                }
                

                return CreateViewFromTaskOid(model);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection values, TaskModel task)
        {
            return CreateViewFromForm(values, task);
        }

        [HttpPost]
        public ActionResult CompleteMgrTask(string completeTask, FormCollection values, TaskModel task)
        {
            // Custom computation

            if (completeTask == Approve)
            {
               
            }
            if (completeTask == Reject)
            {
                
            }

            // Exit from this area and go back to main control
            return RedirectFromArea(completeTask, values);
        }
    }
}
