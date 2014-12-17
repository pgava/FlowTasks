using System.Collections.Generic;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Flow.Tasks.View.Configuration;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class WorkflowOperationsController : BaseApiController
    {

        public WorkflowOperationsController(IFlowTasksService tasksService)
            : base(tasksService)
        {
        }

        public enum WorkflowOperation
        {
            Start
        }

        /// <summary>
        /// Assign Task to User
        /// </summary>
        /// <remarks>
        /// Request sample:
        /// PATCH http://localhost/Flow.tasks.web/api/workflows/start/?woid=&wcode=HolidayWf
        /// Headers
        /// Host: localhost
        /// Content-Type: application/json
        /// Body
        /// [
        ///   {
        ///     "name":"UserName",
        ///     "value":"pnewman",
        ///     "type":"String"
        ///   },
        ///   {
        ///     "name":"UserNote",
        ///     "value":"note",
        ///     "type":"String"
        ///   },
        ///   {
        ///     "name":"HolidayId",
        ///     "value":"1",
        ///     "type":"String"
        ///   }
        /// ]
        /// </remarks>
        /// <returns></returns>
        [HttpPatch]
        public HttpResponseMessage Operations(string op, string woid, string wcode, [FromBody]IEnumerable<WfProperty> props)
        {
            Guid oid;
            if (!string.IsNullOrWhiteSpace(woid) && !Guid.TryParse(woid, out oid))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            try
            {

                if (op.Equals(WorkflowOperation.Start.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var startWorkflowRequest = new StartWorkflowRequest
                    {
                        Domain = ConfigHelper.WorkflowDomain,
                        WorkflowCode = wcode,
                        WfRuntimeValues = props.ToArray()
                    };

                    TasksService.StartWorkflow(startWorkflowRequest);
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