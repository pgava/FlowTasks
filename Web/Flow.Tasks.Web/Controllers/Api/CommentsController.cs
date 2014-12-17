using System;
using System.Collections.Generic;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View.Models;
using System.Web.Http;


namespace Flow.Tasks.Web.Controllers.Api
{
    public class CommentsController : BaseApiController
    {
        public CommentsController(IFlowTasksService tasksService)
            : base(tasksService)
        {
        }

        /// <summary>
        /// Get Comments For Task
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/comments/7D3A5953-0055-463B-81F7-CEFA81809555
        /// </remarks>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of CommentItem</returns>
        [HttpPost]
        public IEnumerable<CommentItem> GetCommentsForTask(string woid)
        {
            var comments = TasksService.GetTraceForWorkflow(new GetTraceForWorkflowRequest
            {
                WorkflowOids = new[] { Guid.Parse(woid) }
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
                            When = t.When,
                            User = t.User,
                            Message = t.Message,
                            Avatar = t.Avatar
                        });
                    }
                }
            }
            return items;
        }

        
    }
}