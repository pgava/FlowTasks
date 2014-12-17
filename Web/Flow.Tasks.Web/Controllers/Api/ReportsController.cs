using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Web.Controllers.Api
{
    [FlowTasksApiAuthorize(UserRoles = new[] { "PM", "MgrPM" })]
    public class ReportsController : BaseApiController
    {
        public ReportsController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument)
            : base(tasksService, docsDocument)
        {
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/reports/usertaskcount?start=&end=&u=&t=
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName("taskcount")]
        public IEnumerable<ReportUserTaskCountInfo> ReportUserTaskCount(string start, string end, [FromUri]string[] u, [FromUri]string[] t)
        {
            try
            {
                var resp = TasksService.ReportUserTaskCount(new ReportUserTaskCountRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Tasks = t,
                    Users = u
                });

                return resp.Report;
            }
            catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                throw new HttpResponseException(result);
            }
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/reports/tasktime?start=&end=&t=&w=
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName("tasktime")]
        public IEnumerable<ReportTaskTimeInfo> ReportTaskTime(string start, string end, [FromUri]string[] t, [FromUri]string[] w)
        {
            try
            {
                var resp = TasksService.ReportTaskTime(new ReportTaskTimeRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Tasks = t,
                    Workflows = w
                });

                return resp.report;
            }
            catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                throw new HttpResponseException(result);
            }
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/reports/workflowtime?start=&end=&t=&w=
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName("workflowtime")]
        public IEnumerable<ReportWorkflowTimeInfo> ReportWorkflowTime(string start, string end, [FromUri]string[] w)
        {
            try
            {
                var resp = TasksService.ReportWorkflowTime(new ReportWorkflowTimeRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Workflows = w
                });

                return resp.Report;
            }
            catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                throw new HttpResponseException(result);
            }
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/reports/usertasks?start=&end=&u=
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName("usertasks")]
        public IEnumerable<ReportUserTasksInfo> ReportUserTasks(string start, string end, [FromUri]string[] u)
        {
            try
            {
                var resp = TasksService.ReportUserTasks(new ReportUserTasksRequest
                {
                    End = ParseString(end),
                    Start = ParseString(start),
                    Users = u
                });

                return resp.Report;
            }
            catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                throw new HttpResponseException(result);
            }
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