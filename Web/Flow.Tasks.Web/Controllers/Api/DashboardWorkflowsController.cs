using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Flow.Tasks.Web.Controllers.Api
{
    [FlowTasksApiAuthorize(UserRoles = new[] { "Admin" })]
    public class DashboardWorkflowsController : BaseApiController
    {

        public enum WorkflowOperation
        {
            Terminate,
            Delete,
            Restart
        }


        public DashboardWorkflowsController(IFlowTasksService tasksService)
            : base(tasksService)
        {

        }

        [HttpGet]
        public DashboardWorkflowModel GetWorkflow(string woid)
        {
            var workflows = TasksService.GetWorkflows(new GetWorkflowRequest
            {
                WorkflowId = woid
            });

            return workflows.WorkflowInfos.Select(w => new DashboardWorkflowModel
            {
                code = w.WorkflowCode,
                end = w.CompletedOn,
                expanded = "true",
                loaded = "true",
                parentid = ConvertOidToString(w.ParentWorkflowOid),
                parent = Position(workflows.WorkflowInfos, ConvertOidToString(w.ParentWorkflowOid)),
                select = "false",
                start = w.StartedOn,
                status = w.Status,
                wfid = w.WorkflowId
            }).FirstOrDefault();

        }

        /// <summary>
        /// Get Workflows
        /// </summary>
        /// <remarks>
        /// GET http://localhost/Flow.tasks.web/api/dashboardworkflows?woid=&wcode=&isActive=&start=&end=&pageIndex=0&pageSize=10
        /// </remarks>
        /// <param name="woid"></param>
        /// <param name="wcode"></param>
        /// <param name="isActive"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public DashboardWorkflowsModel GetWorkflows(string woid, string wcode, string isActive, string start, string end, int pageIndex, int pageSize)
        {
            DateTime startedFrom;
            DateTime startedTo;
            var isStartedFrom = DateTime.TryParse(start, out startedFrom);
            var isStaredTo = DateTime.TryParse(end, out startedTo);

            var workflows = TasksService.GetWorkflows(new GetWorkflowRequest
            {
                WorkflowId = woid,
                IsActive = isActive == "true",
                WorkflowCode = wcode,
                StartedFrom = !isStartedFrom ? null : new DateTime?(startedFrom),
                StartedTo = !isStaredTo ? null : new DateTime?(startedTo),
                PageIndex = pageIndex,
                PageSize = pageSize
            });

            // Build list for Grid
            var model = workflows.WorkflowInfos.Select(w => new DashboardWorkflowModel
            {                
                code = w.WorkflowCode,
                end = w.CompletedOn,
                expanded = "true",
                loaded = "true",
                parentid = ConvertOidToString(w.ParentWorkflowOid),
                parent = Position(workflows.WorkflowInfos, ConvertOidToString(w.ParentWorkflowOid)),
                select = "false",
                start = w.StartedOn,
                status = w.Status,
                wfid = w.WorkflowId
            }).ToList();

            foreach (var item in model)
            {
                item.isLeaf = model.Count(w => w.parentid == item.wfid) == 0 ? "true" : "false";

                item.level = Level(model, item).ToString(CultureInfo.InvariantCulture);
            }

            return new DashboardWorkflowsModel
                {
                    TotalWorkflows = workflows.TotalWorkflows,
                    WorkflowCodes = workflows.WorkflowCodes,
                    Workflows = model
                };
        }

        /// <summary>
        /// Operations
        /// </summary>
        /// <remarks>
        /// PATCH http://localhost/Flow.tasks.web/api/dashboardworkflows?op=Delete
        /// PATCH http://localhost/Flow.tasks.web/api/dashboardworkflows?op=Restart
        /// PATCH http://localhost/Flow.tasks.web/api/dashboardworkflows?op=Terminate
        ///
        /// Body
        /// ["7D3A5953-0055-463B-81F7-CEFA81809555"]
        /// 
        /// </remarks>
        /// <param name="woids"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        [HttpPatch]
        public HttpResponseMessage Operations([FromBody]IEnumerable<string> woids, string op)
        {
            var messages = new List<ErrorMessage>();

            if (op == WorkflowOperation.Delete.ToString())
            {
                foreach (var o in woids)
                {
                    var resp = TasksService.DeleteWorkflow(new ControlWorkflowRequest
                    {
                        WorkflowOid = o
                    });

                    if (resp.Message != "OK")
                    {
                        messages.Add(new ErrorMessage {Id = o, Message = resp.Message});
                    }

                }
            }
            if (op == WorkflowOperation.Restart.ToString())
            {
                foreach (var o in woids)
                {
                    var resp = TasksService.ReStartWorkflow(new ReStartWorkflowRequest
                    {
                        OldWorkflowId = o
                    });

                    if (resp.Message != "OK")
                    {
                        messages.Add(new ErrorMessage { Id = o, Message = resp.Message });
                    }
                }
            }
            if (op == WorkflowOperation.Terminate.ToString())
            {
                foreach (var o in woids)
                {
                    var resp = TasksService.CancelWorkflow(new ControlWorkflowRequest
                    {
                        WorkflowOid = o
                    });

                    if (resp.Message != "OK")
                    {
                        messages.Add(new ErrorMessage { Id = o, Message = resp.Message });
                    }
                }
            }

            var json = JsonConvert.SerializeObject(
                messages,
                Formatting.Indented,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StringContent(json, Encoding.UTF8, "text/plain");
            return result;
        }


        /// <summary>
        /// Position of the workflow in the list
        /// </summary>
        /// <param name="infos">WorkflowInfos</param>
        /// <param name="oid">Oid</param>
        /// <returns>position</returns>
        private string Position(WorkflowInfos infos, string oid)
        {
            if (!string.IsNullOrWhiteSpace(oid))
            {
                int pos = 1;
                for (int idx = 0; idx < infos.Count(); idx++)
                {
                    if (infos[idx].WorkflowId == oid) return pos.ToString(CultureInfo.InvariantCulture);
                    pos++;
                }
            }

            return "null";
        }

        /// <summary>
        /// Level of parent/child relationship
        /// </summary>
        /// <param name="models">List of DashboardWorkflowModel</param>
        /// <param name="item">DashboardWorkflowModel</param>
        /// <returns>level</returns>
        private int Level(IEnumerable<DashboardWorkflowModel> models, DashboardWorkflowModel item)
        {
            if (string.IsNullOrWhiteSpace(item.parentid))
            {
                return 0;
            }
            var dashboardWorkflowModels = models as IList<DashboardWorkflowModel> ?? models.ToList();
            item = dashboardWorkflowModels.First(w => w.wfid == item.parentid);
            return Level(dashboardWorkflowModels, item) + 1;
        }

        /// <summary>
        /// Convert Oid ToString
        /// </summary>
        /// <param name="oid">Oid</param>
        /// <returns>empty string or oid</returns>
        private string ConvertOidToString(Guid oid)
        {
            if (oid == Guid.Empty)
                return string.Empty;

            return oid.ToString();
        }
    }
}