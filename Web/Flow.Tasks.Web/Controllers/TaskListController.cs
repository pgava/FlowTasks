using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Flow.Tasks.View;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Models;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;

namespace Flow.Tasks.Web.Controllers
{

    [Authorize]
    [HandleError]
    public class TaskListController : BaseController
    {
        /// <summary>
        /// Complete Flags
        /// </summary>
        const string DEFAULT_ACCEPT_FLAG = "Accept";
        const string DEFAULT_REJECT_FLAG = "Reject";
        const string ACCEPT_VALUES = "TaskAcceptFlag";
        const string REJECT_VALUES = "TaskRejectFlag";
        
        /// <summary>
        /// Parameters flag
        /// </summary>
        const string PARAMETERS = "TaskParameter";

        public TaskListController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        /// <summary>
        /// Index action
        /// </summary>
        /// <param name="completeTask">Name of button was clicled</param>
        /// <param name="values">Values posted back</param>
        /// <returns>ActionResult</returns>
        public ActionResult Index(string completeTask, FormCollection values)
        {
            ReadTempDataFromRedirect(ref completeTask, ref values);

            var task = CreateTaskFromValues(values);

            var completeFlags = CreateCompleteFlags(values);

            if (completeFlags.Contains(completeTask))
            {
                return (CompleteTask(task, completeTask));
            }

            // If they've submitted the form without a submitButton,  
            // just return the view again. 
            return View(GetTasksList(GetFilterValues(task)));
        }

        /// <summary>
        /// Read TempData From Redirect
        /// </summary>
        /// <param name="completeTask">completeTask</param>
        /// <param name="values">values</param>
        private void ReadTempDataFromRedirect(ref string completeTask, ref FormCollection values)
        {
            if (TempData["CompleteTask"] != null)
            {
                completeTask = TempData["CompleteTask"] as string;
            }
            if (TempData["FormValues"] != null)
            {
                values = TempData["FormValues"] as FormCollection;
            }
        }

        /// <summary>
        /// Create Complete Flags
        /// <remarks>
        /// Read all the possible values the view uses to approve/reject task. By convention 
        /// the view can put these values in 2 hidden fields called TaskAcceptFlag & TaskRejectFlag
        /// </remarks>
        /// </summary>
        /// <param name="values">FormCollection</param>
        /// <returns>List of string</returns>
        private IEnumerable<string> CreateCompleteFlags(FormCollection values)
        {
            List<string> completeResults = new List<string>();

            // Add defaults
            completeResults.Add(DEFAULT_ACCEPT_FLAG);
            completeResults.Add(DEFAULT_REJECT_FLAG);

            // Get all the other values from post back values
            var accept = values.GetValue(ACCEPT_VALUES);
            var reject = values.GetValue(REJECT_VALUES);
            if (accept != null)
            {
                completeResults.AddRange(accept.AttemptedValue.ToString(CultureInfo.InvariantCulture).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            if (reject != null)
            {
                completeResults.AddRange(reject.AttemptedValue.ToString(CultureInfo.InvariantCulture).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }

            return completeResults;
        }

        /// <summary>
        /// Create Filter From Values
        /// </summary>
        /// <param name="values"></param>
        /// <returns>FilterModel</returns>
        private FilterModel CreateFilterFromValues(FormCollection values)
        {
            return new FilterModel
            {
                DisplayMethod = values["Filter.DisplayMethod"],
                Filter = values["Filter.Filter"],
                OrderMethod = values["Filter.OrderMethod"],
                CurrentPage = values["Filter.CurrentPage"],
                FilteredPages = values["Filter.FilteredPages"],
                TotalPages = values["Filter.TotalPages"],
                Domain = values["Filter.Domain"],
                MaxTasks = values["Filter.MaxTasks"],
                DomainList = GetDomainsForUser()
            };
        }

        /// <summary>
        /// Create Comment From Values
        /// </summary>
        /// <param name="values"></param>
        /// <returns>CommentModel</returns>
        private CommentModel CreateCommentFromValues(FormCollection values)
        {
            return new CommentModel
            {
                Status = CommentModel.CommentStatus.Optional.ToString(),
                TaskComment = values["Comment.TaskComment"],
                Comments = null
            };
        }

        /// <summary>
        /// Create Parameters From Values
        /// </summary>
        /// <param name="values">Values</param>
        /// <returns>List of PropertyInfo</returns>
        private PropertyInfo[] CreateParametersFromValues(FormCollection values)
        {
            var parameters = new List<PropertyInfo>();
            foreach (var key in values.Keys)
            {
                var propertyKey = key.ToString();
                if (propertyKey.Contains(PARAMETERS))
                {
                    parameters.Add(new PropertyInfo
                    {
                        Name = propertyKey.Replace(PARAMETERS, string.Empty),
                        Value = values[propertyKey]
                    });
                }
            }
            return parameters.ToArray();
        }

        /// <summary>
        /// Create Task From Values
        /// </summary>
        /// <remarks>
        /// The view can add parameters to the task using fields with name containing
        /// "TaskParameters" flag.
        /// </remarks>
        /// <param name="values">FormCollection</param>
        /// <returns>TaskModel</returns>
        private TaskModel CreateTaskFromValues(FormCollection values)
        {
            if (values.Count == 0) return null;

            return new TaskModel
            {
                Description = values["Description"],
                DefaultResult = values["DefaultResult"],
                Expires = values["Expires"],
                IsAssigned = values["IsAssigned"].ToUpper() == "TRUE",
                TaskCode = values["TaskCode"],
                TaskCorrelationId = int.Parse(values["TaskCorrelationId"]),
                TaskOid = values["TaskOid"],
                Title = values["Title"],
                UiCode = values["UiCode"],
                WorkflowOid = values["WorkflowOid"],
                Filter = CreateFilterFromValues(values),
                Comment = CreateCommentFromValues(values),
                Parameters = CreateParametersFromValues(values)
            };
        }


        /// <summary>
        /// Complete Task
        /// </summary>
        /// <param name="task">TaskModel</param>
        /// <param name="result">Result for the task</param>
        /// <returns>ActionResult</returns>
        private ActionResult CompleteTask(TaskModel task, string result)
        {
            ApproveTask(task.WorkflowOid, task.TaskOid, task.Comment.TaskComment,
                task.TaskCorrelationId, task.TaskCode, task.Parameters,
                result);

            return Content("");

            //return Redirect("/#/tasks");

            return View(GetTasksList(task.Filter));
        }


        /// <summary>
        /// Approve Task
        /// </summary>
        /// <param name="workflowOid"></param>
        /// <param name="taskOid"></param>
        /// <param name="message"></param>
        /// <param name="correlationId"></param>
        /// <param name="taskCode"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        private void ApproveTask(string workflowOid, string taskOid, string message,
            int correlationId, string taskCode, IEnumerable<PropertyInfo> parameters, string result)
        {
            TasksService.AddTraceToWorkflow(new AddTraceToWorkflowRequest
            {
                WorkflowOid = workflowOid,
                TaskOid = taskOid,
                User = HttpContext.User.Identity.Name,
                Message = message
            });

            TasksService.ApproveTask(new ApproveTaskRequest
            {
                TaskId = taskOid,
                CorrelationId = correlationId,
                TaskCode = taskCode,
                Result = result,
                UserName = HttpContext.User.Identity.Name,
                WorkflowId = workflowOid,
                Parameters = new PropertyInfos(parameters)
            });

        }

        /* GetTasksList
         * TODO: To improve performance we should get the tasks to display
         * from ther server without  getting all the records all the time.
         */

        /// <summary>
        /// Get Tasks List
        /// </summary>
        /// <param name="filterModel">FilterModel</param>
        /// <returns>TaskListModel</returns>
        private TaskListModel GetTasksList(FilterModel filterModel)
        {
            var getNextTasksForUserRequest = new GetNextTasksForUserRequest
            {
                User = HttpContext.User.Identity.Name,
                WorkflowOid = Guid.Empty,
                Domain = filterModel.Domain == Domains.All ? string.Empty : filterModel.Domain,
            };

            GetNextTasksForUserResponse tasks = TasksService.GetNextTasksForUser(getNextTasksForUserRequest);

            var filter = new TaskFilter(filterModel);
            var trimedTasks = filter.RunSearch(tasks.Tasks);

            var taskList = CreateTaskList(filter, trimedTasks);

            return new TaskListModel { Tasks = taskList };
        }

        /// <summary>
        /// Create Task List
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="trimedTasks">TrimedTasks</param>
        /// <returns>List of TaskModel</returns>
        private List<TaskModel> CreateTaskList(TaskFilter filter, IEnumerable<TaskInfo> trimedTasks)
        {
            var taskList = new List<TaskModel>();

            foreach (var t in trimedTasks)
            {
                var expires = t.ExpiryDate.HasValue ? t.ExpiryDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "none";

                taskList.Add(new TaskModel
                {
                    Expires = expires,
                    UiCode = t.UiCode,
                    TaskCode = t.TaskCode,
                    Title = t.Title,
                    Description = t.Description,
                    DefaultResult = t.DefaultResult,
                    TaskOid = t.TaskOid.ToString(),
                    WorkflowOid = t.WorkflowOid.ToString(),
                    TaskCorrelationId = t.TaskCorrelationId,
                    IsAssigned = !string.IsNullOrWhiteSpace(t.AcceptedBy),
                    Filter = filter.GetFilterModel(),
                    Comment = new CommentModel
                    {
                        Comments = GetCommentsForTask(t.WorkflowOid.ToString()),
                        Status = CommentModel.CommentStatus.Optional.ToString(),
                        TaskComment = string.Empty
                    },
                    Parameters = GetTaskParameters(t.TaskOid),
                    Documents = GetAttachedDocuments(t.TaskOid)
                });
            }

            return taskList;
        }

    }

    
}
