using Flow.Tasks.Contract.Message;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;
using Flow.Tasks.View.Models;

namespace Flow.Tasks.Web.Models
{
    public class ApiModelFactory
    {
        private UrlHelper _urlHelper;
        
        public ApiModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);

        }

        public TaskApiModel Create(TaskInfo task, IEnumerable<DocumentModel> docs)
        {
            return new TaskApiModel
            {
                Url = _urlHelper.Link("Task", new { toid = task.TaskOid.ToString()}),
                Task = CreateTask(task, docs)
            };
        }

        public TaskApiModel Create(TaskInfo task, IEnumerable<DocumentModel> docs, IEnumerable<CommentItem> coms, IEnumerable<PropertyInfo> props)
        {
            return new TaskApiModel
            {
                Url = _urlHelper.Link("Task", new { toid = task.TaskOid.ToString() }),
                Task = CreateTask(task, docs, coms, props)
            };
        }

        private View.Models.TaskModel CreateTask(TaskInfo t, IEnumerable<DocumentModel> docs)
        {
            return CreateTask(t, docs, new List<CommentItem>(), new List<PropertyInfo>());
        }

        private View.Models.TaskModel CreateTask(TaskInfo t, IEnumerable<DocumentModel> docs, IEnumerable<CommentItem> coms, IEnumerable<PropertyInfo> props)
        {
            var expires = t.ExpiryDate.HasValue ? t.ExpiryDate.Value.ToString("f") : "none";

            return new View.Models.TaskModel
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
                Documents = docs.ToArray(),
                Comment = new CommentModel
                {
                    Comments = coms
                },
                Parameters = props.ToArray()
            };
        }
    }
}