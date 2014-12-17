using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Models;
using Flow.Tasks.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class TasksController : BaseApiController
    {
        public TasksController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument)
            : base(tasksService, docsDocument)
        {
        }

        /// <summary>
        /// Get Tasks For User
        /// </summary>
        /// <remarks>
        /// Request sample:
        /// GET http://localhost/Flow.tasks.web/api/tasks?user=cgrant&pageIndex=0&pageSize=5&searchFor=
        /// Headers
        /// Host: localhost
        /// Content-Type: application/json; charset=utf-8
        /// Accept: application/json
        /// </remarks>
        /// <param name="user">User</param>
        /// <param name="searchFor"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>List of tasks</returns>
        [HttpGet]
        public IEnumerable<TaskApiModel> GetTasksForUser(string user, string searchFor, int pageSize, int pageIndex)
        {

            return TasksService.GetNextTasksForUser(new GetNextTasksForUserRequest
            {
                User = user,
                Domain = string.Empty,
                WorkflowOid = Guid.Empty,
                SearchFor = searchFor,
                PageIndex = pageIndex,
                PageSize = pageSize
            }).Tasks
            .Select(t => TheModelFactory.Create(t, GetAttachedDocuments(t.TaskOid)));

        }

        /// <summary>
        /// Get Attached Documents
        /// </summary>
        /// <param name="taskOid">TaskOid</param>
        /// <returns>List of DocumentModel</returns>
        private IEnumerable<DocumentModel> GetAttachedDocuments(Guid taskOid)
        {
            var properties = TasksService.GetPropertiesForTask(
                new GetPropertiesForTaskRequest { TaskOid = taskOid });

            var docs = new List<DocumentModel>();
            var oids = new List<Guid>();

            foreach (var prop in properties.Properties)
            {
                if (prop.Type == PropertyType.FlowDoc.ToString())
                {
                    oids.Add(Guid.Parse(prop.Value));
                }
            }

            var res = DocsDocument.DocumentInfos(oids.ToArray());

            return res.Select(d => new DocumentModel
            {
                DocumentOid = d.OidDocument.ToString(),
                DocumentName = d.DocumentName
            }).ToList();
        }

        /// <summary>
        /// Get Comments For Task
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of CommentItem</returns>
        private IEnumerable<CommentItem> GetCommentsForTask(string workflowOid)
        {
            var comments = TasksService.GetTraceForWorkflow(new GetTraceForWorkflowRequest
            {
                WorkflowOids = new[] { Guid.Parse(workflowOid) }
            });

            List<CommentItem> items = new List<CommentItem>();
            if (comments.Traces != null)
            {
                foreach (var t in comments.Traces)
                {
                    if (t.Action == ActionTrace.UserMessage.ToString())
                    {
                        items.Add(new CommentItem
                        {
                            Avatar = t.Avatar,
                            When = t.When,
                            User = t.User,
                            Message = t.Message
                        });
                    }
                }
            }
            return items;
        }

        /// <summary>
        /// Get Task Parameters
        /// </summary>
        /// <param name="taskOid">TaskOid</param>
        /// <returns>List of PropertyInfo</returns>
        private PropertyInfo[] GetTaskParameters(Guid taskOid)
        {
            var properties = TasksService.GetPropertiesForTask(new GetPropertiesForTaskRequest
            {
                TaskOid = taskOid
            });

            return properties.Properties.ToArray();
        }


        /// <summary>
        /// Get Tasks For Workflow
        /// </summary>
        /// <remarks>
        /// Request sample:
        /// GET http://localhost/flow.tasks.web/api/tasks?woid=1ea8d579-4879-4e07-a601-00b5b5caf45b
        /// Headers
        /// Host: localhost
        /// Content-Type: application/json; charset=utf-8
        /// Accept: application/json
        /// </remarks>
        /// <param>Workflow Oid
        ///     <name>woid</name>
        /// </param>
        /// <param name="woid"></param>
        /// <returns>List of tasks</returns>
        [HttpGet]
        public IEnumerable<TaskApiModel> GetTasksForWorkflow(string woid)
        {
            Guid oid;
            if (string.IsNullOrWhiteSpace(woid) || !Guid.TryParse(woid, out oid))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            
            return TasksService.GetNextTasksForWorkflow(new GetNextTasksForWorkflowRequest
            {
                WorkflowOid = oid
            }).Tasks
            .Select(t => TheModelFactory.Create(t, GetAttachedDocuments(t.TaskOid)));
        }

        /// <summary>
        /// Get Task
        /// </summary>
        /// <remarks>
        /// Request sample:
        /// GET http://localhost/Flow.Tasks.Web/api/tasks/63889ce7-92a1-47f0-8ff8-1914197738ef
        /// Headers
        /// Host: localhost
        /// Content-Type: application/json; charset=utf-8
        /// Accept: application/json
        /// </remarks>
        /// <param name="toid">Task Oid</param>
        /// <returns>Task</returns>
        [HttpGet]
        public TaskApiModel GetTask(string toid)
        {

            var task = TasksService.GetTask(new GetTaskRequest { TaskOid = Guid.Parse(toid) });
            var docs = GetAttachedDocuments(task.Task.TaskOid);
            var comments = GetCommentsForTask(task.Task.WorkflowOid.ToString());
            var parameters = GetTaskParameters(task.Task.TaskOid);

            return TheModelFactory.Create(task.Task, docs, comments, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetTaskCount(string toid, string user)
        {
            return TasksService.GetTaskCount(new GetTaskCountRequest {User = user}).Count;
        }
    }
}