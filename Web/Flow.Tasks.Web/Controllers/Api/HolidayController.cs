using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class HolidayController : BaseApiController
    {
        public HolidayController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument)
            : base(tasksService, docsDocument)
        {
        }

        /// <summary>
        /// Apply holidays
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/holiday/cgrant
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        [HttpPost] public HttpResponseMessage ApplyHoliday(string name, IEnumerable<string>dates)
        {
            try
            {
                var resp = TasksService.ApplyHoliday(new ApplyHolidayRequest {User = name, Type = 2, Holiday = dates});
                
                var json = JsonConvert.SerializeObject(
                    new {resp.HolidayId},
                    Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                    );
                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StringContent(json, Encoding.UTF8, "text/plain");

                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Get Holiday
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/holiday/cgrant
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<HolidayInfo> GetHoliday(string name, int year)
        {
            try
            {
                var resp = TasksService.GetHolidayForUser(new GetHolidayForUserRequest {User = name, Year = year});
                return resp.Holidays;
            }
            catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                throw new HttpResponseException(result);
            }
        }

        /// <summary>
        /// Remove holiday
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/holiday/
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="holidayId"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RemoveHoliday(string name, int holidayId, IEnumerable<string> dates)
        {
            try
            {
                //1. search if exist wf
                var workflows = TasksService.SearchWorkflows(new SearchWorkflowsRequest
                {
                    Domain = string.Empty,
                    WorkflowCode = "HolidayWf",
                    Properties = new PropertyInfos(new List<PropertyInfo>
                    {
                        new PropertyInfo
                        {
                            Name = "UserName",
                            Value = name
                        },
                        new PropertyInfo
                        {
                            Name = "HolidayId",
                            Value = holidayId.ToString()
                        }
                    })
                });

                if (workflows.WorkflowInfos.Any())
                {
                    //2. If workflow in progress terminate
                    if (workflows.WorkflowInfos[0].Status == WorkflowStatusType.InProgress.ToString())
                    {
                        var resp = TasksService.CancelWorkflow(new ControlWorkflowRequest
                        {
                            WorkflowOid = workflows.WorkflowInfos[0].WorkflowId
                        });
                    }
                    else
                    {
                        //3. else start a new wf or send a notification
                        TasksService.CreateNotification(new CreateNotificationRequest
                        {
                            NotificationInfo = new NotificationInfo
                            {
                                AssignedToUsers = "{r.HR}",
                                TaskOid = Guid.NewGuid(),
                                WorkflowOid = Guid.Parse(workflows.WorkflowInfos[0].WorkflowId),
                                Title = string.Format("Holiday cancellation for user {0}", name),
                                Description = string.Format("The holiday previously requested by user {0} has been cancelled. Holiday details: {1}", name, string.Join(",", dates))
                            }
                        });
                    }
                }

                var removeHoliday = TasksService.RemoveHoliday(new RemoveHolidayRequest
                {
                    HolidayId = holidayId,
                    User = name
                });

                if (!removeHoliday.IsRemoved)
                {
                    var result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                    result.Content = new StringContent("Could not remove workflow. Contact System Administrator.", Encoding.UTF8, "text/plain");
                    return result;
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }

}