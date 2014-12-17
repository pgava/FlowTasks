using System;
using System.Collections.Generic;
using System.Web.Http;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Web.Models;

namespace Flow.Tasks.Web.Controllers.Api
{
    [FlowTasksApiAuthorize(UserRoles = new[] { "Admin" })]
    public class DashboardDetailsController : BaseApiController
    {
        public DashboardDetailsController(IFlowTasksService tasksService)
            : base(tasksService)
        {

        }

        /// <summary>
        /// GetDetails
        /// </summary>
        /// <remarks>
        /// GET http://localhost/Flow.tasks.web/api/dashboarddetails/7d3a5953-0055-463b-81f7-cefa81809555/
        /// </remarks>
        /// <param name="woid"></param>
        /// <returns></returns>
        [HttpGet]
        public DashboardShowWorkflowModel GetDetails(string woid)
        {
            return new DashboardShowWorkflowModel
            {
                Properties = GetProperties(woid),
                Tasks = GetTasks(woid),
                Traces = GetTraces(woid)
            };
        }

        private TaskModel GetTasks(string workflowOid)
        {
            var task = new TaskModel();

            var tasks = TasksService.GetNextTasksForWorkflow(new GetNextTasksForWorkflowRequest
            {
                WorkflowOid = Guid.Parse(workflowOid)
            });

            List<TaskItem> items = new List<TaskItem>();
            foreach (var t in tasks.Tasks)
            {
                items.Add(new TaskItem
                {
                    Code = t.TaskCode,
                    Title = t.Title,
                    Description = t.Description,
                    AcceptedBy = string.IsNullOrWhiteSpace(t.AcceptedBy) ? string.Empty : t.AcceptedBy,
                    TaskId = t.TaskOid.ToString()
                });
            }

            task.Tasks = items;

            return task;
        }

        private PropertyModel GetProperties(string workflowOid)
        {
            var property = new PropertyModel();

            var properties = TasksService.GetWorkflowParameters(new GetWorkflowParametersRequest
            {
                WorkflowOid = workflowOid
            });

            List<PropertyItem> items = new List<PropertyItem>();
            foreach (var p in properties.Properties)
            {
                items.Add(new PropertyItem
                {
                    Name = p.Name,
                    Value = p.Value,
                    Type = p.Type
                });
            }

            property.Properties = items;
            return property;
        }

        private TraceModel GetTraces(string workflowOid)
        {
            var trace = new TraceModel();

            var traces = TasksService.GetTraceForWorkflow(new GetTraceForWorkflowRequest
            {
                WorkflowOids = new[] {Guid.Parse(workflowOid)}
            });

            List<TraceItem> items = new List<TraceItem>();
            foreach (var t in traces.Traces)
            {
                items.Add(new TraceItem
                {
                    When = t.When,
                    Action = t.Action,
                    User = t.User,
                    Message = t.Message
                });
            }

            trace.Traces = items;

            return trace;
        }

    }
}