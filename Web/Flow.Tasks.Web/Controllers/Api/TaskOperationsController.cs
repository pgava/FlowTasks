using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class TaskOperationsController : BaseApiController
    {

        public TaskOperationsController(IFlowTasksService tasksService)
            : base(tasksService)
        {
        }

        public enum TaskOperation
        {
            Assign,
            Complete,
            Approve,
            GiveBack
        }

        /// <summary>
        /// Assign Task to User
        /// </summary>
        /// <remarks>
        /// Request sample:
        /// PATCH http://localhost/Flow.tasks.web/api/tasks/1ea8d579-4879-4e07-a601-00b5b5caf45b/assign
        /// PATCH http://localhost/Flow.tasks.web/api/tasks/1ea8d579-4879-4e07-a601-00b5b5caf45b/complete
        /// PATCH http://localhost/Flow.tasks.web/api/tasks/1ea8d579-4879-4e07-a601-00b5b5caf45b/giveback
        /// PATCH http://localhost/Flow.tasks.web/api/tasks/1ea8d579-4879-4e07-a601-00b5b5caf45b/approve
        /// Headers
        /// Host: localhost
        /// Content-Type: application/json
        /// Body
        /// {
        /// "acceptedBy": "cgran"
        /// }
        /// </remarks>
        /// <param name="toid">Task Oid</param>
        /// <param name="op">Operation: assign/complete/giveback</param>
        /// <param name="task">Task</param>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPatch]
        public HttpResponseMessage Operations(string toid, string op, [FromBody]TaskInfo task, string result = "", string message = "")
        {
            Guid oid;
            if (!Guid.TryParse(toid, out oid))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            try
            {

                if (op.Equals(TaskOperation.Assign.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    TasksService.AssignTaskTo(new AssignTaskToRequest
                        {
                            User = task.AcceptedBy,
                            TaskOid = oid
                        });
                }
                else if (op.Equals(TaskOperation.Complete.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    TasksService.CompleteTask(new CompleteTaskRequest
                        {
                            TaskId = toid,
                            User = task.AcceptedBy,
                            Result = result
                        });
                }
                else if (op.Equals(TaskOperation.GiveBack.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    TasksService.GiveBackTask(new GiveBackTaskRequest
                        {
                            TaskOid = oid
                        });
                }
                else if (op.Equals(TaskOperation.Approve.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var properties = TasksService.GetPropertiesForTask(new GetPropertiesForTaskRequest
                        {
                            TaskOid = Guid.Parse(toid)
                        });

                    var taskFromDb = TasksService.GetTask(new GetTaskRequest {TaskOid = Guid.Parse(toid)});

                    // Do I need this?
                    // If this is necessary think of notification
                    /*
                    TasksService.AddTraceToWorkflow(new AddTraceToWorkflowRequest
                        {
                            WorkflowOid = taskFromDb.Task.WorkflowOid.ToString(),
                            TaskOid = toid,
                            User = task.AcceptedBy,
                            Message = message
                        });
                    */

                    TasksService.ApproveTask(new ApproveTaskRequest
                        {
                            TaskId = toid,
                            CorrelationId = taskFromDb.Task.TaskCorrelationId,
                            TaskCode = taskFromDb.Task.TaskCode,
                            Result = result,
                            UserName = task.AcceptedBy,
                            WorkflowId = taskFromDb.Task.WorkflowOid.ToString(),
                            Parameters = new PropertyInfos(properties.Properties)
                        });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}