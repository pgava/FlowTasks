using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Ninject;

namespace Flow.Tasks.Service
{
    /// <summary>
    /// FlowTasksService
    /// </summary>
    public class FlowTasksService : IFlowTasksService
    {
        [Inject]
        public ITask _task { get; set; }

        [Inject]
        public IWorkflow _workflow { get; set; }

        [Inject]
        public ITopic _messenger { get; set; }

        [Inject]
        public IHoliday _holiday { get; set; }

        [Inject]
        public IFlowTasksProxyManager _proxyManager { get; set; }

        public FlowTasksService(ITask task, IWorkflow workflow, ITopic messenger, IHoliday holiday, IFlowTasksProxyManager proxyManager)
        {
            _task = task;
            _workflow = workflow;
            _messenger = messenger;
            _holiday = holiday;
            _proxyManager = proxyManager;
        }

        #region Workflow

        /// <summary>
        /// Start Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>StartWorkflowResponse</returns>
        public StartWorkflowResponse StartWorkflow(StartWorkflowRequest request)
        {
            var proxy = _proxyManager.GetProxyForWorkflow(request.WorkflowCode);
            var resp = proxy.StartWorkflow1(request);

            var toDispose = proxy as System.ServiceModel.ClientBase<IFlowTasksOperations>;
            if (toDispose != null) toDispose.Close();

            return resp;
        }

        /// <summary>
        /// Add Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>AddWorkflowResponse</returns>
        public AddWorkflowResponse AddWorkflow(AddWorkflowRequest request)
        {
            _workflow.AddWorkflow(request.WorkflowCode, request.ServiceUrl, request.BindingConfiguration, request.ServiceEndpoint);
            return new AddWorkflowResponse { Success = true };
        }

        /// <summary>
        /// Update Workflow Parameters
        /// </summary>
        /// <param name="request">Request</param>
        public void UpdateWorkflowParameters(UpdateWorkflowParametersRequest request)
        {
            _workflow.UpdateWorkflowParameters(string.IsNullOrWhiteSpace(request.WorkflowOid) ? Guid.Empty : Guid.Parse(request.WorkflowOid),
                string.IsNullOrWhiteSpace(request.TaskOid) ? Guid.Empty : Guid.Parse(request.TaskOid),
                GetParametersFromRequest(request));
        }

        /// <summary>
        /// Get Parameters From Request
        /// </summary>
        /// <param name="request">StartWorkflowRequest</param>
        /// <returns>List of PropertyInfo</returns>
        private IEnumerable<PropertyInfo> GetParametersFromRequest(UpdateWorkflowParametersRequest request)
        {
            var res = new List<PropertyInfo>();
            if (request.WfRuntimeValues == null) return res;

            foreach (var d in request.WfRuntimeValues)
            {
                res.Add(new PropertyInfo { Name = d.Name, Value = d.Value, Type = d.Type });
            }

            return res;
        }

        /// <summary>
        /// Sketch Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>SketchWorkflowResponse</returns>
        public SketchWorkflowResponse SketchWorkflow(SketchWorkflowRequest request)
        {
            var oid = string.IsNullOrWhiteSpace(request.XamlxOid) ? Guid.Empty : Guid.Parse(request.XamlxOid);
            _workflow.SketchWorkflow(request.Name, oid, request.ChangedBy, request.Status);
            return new SketchWorkflowResponse { Success = true };
        }

        /// <summary>
        /// Get Sketch For Filter
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetSketchForFilterResponse</returns>
        public GetSketchForFilterResponse GetSketchForFilter(GetSketchForFilterRequest request)
        {
            return new GetSketchForFilterResponse { Sketches = new SketchInfos(_workflow.GetSketchForFilter(request.Name, request.Statuses)) };
        }

        /// <summary>
        /// Get Workflow Parameters
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowParametersResponse</returns>
        public GetWorkflowParametersResponse GetWorkflowParameters(GetWorkflowParametersRequest request)
        {
            return new GetWorkflowParametersResponse
            {
                Properties = new PropertyInfos(_workflow.GetWorkflowParameters(Guid.Parse(request.WorkflowOid)))
            };
        }

        /// <summary>
        /// Search For Tasks
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowParametersResponse</returns>
        public SearchForTasksResponse SearchForTasks(SearchForTasksRequest request)
        {
            return new SearchForTasksResponse
            {
                Tasks = _task.SearchForTasks(request.TaskCode, request.AcceptedBy, request.Properties).ToArray()
            };
        }

        /// <summary>
        /// Get Stats For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetStatsForUserResponse</returns>
        public GetStatsForUserResponse GetStatsForUser(GetStatsForUserRequest request)
        {
            int completedTotal;
            int todoTotal;
            var completed = _task.GetTasksCompleted(request.User, out completedTotal);
            var todo = _task.GetTasksToDo(request.User, out todoTotal);

            return new GetStatsForUserResponse
            {
                TaskCompleted = completedTotal,
                TaskToDo = todoTotal,
                TasksCompleted = completed,
                TasksToDo = todo
            };
        }

        /// <summary>
        /// Get Trace For Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetTraceForWorkflowResponse</returns>
        public GetTraceForWorkflowResponse GetTraceForWorkflow(GetTraceForWorkflowRequest request)
        {
            return new GetTraceForWorkflowResponse
            {
                Traces = new WorkflowTraceInfos(_workflow.GetTraceForWorkflow(request.WorkflowOids))
            };
        }

        /// <summary>
        /// Add Trace To Workflow
        /// </summary>
        /// <param name="request">Request</param>
        public void AddTraceToWorkflow(AddTraceToWorkflowRequest request)
        {
            _workflow.AddTraceToWorkflow(Guid.Parse(request.WorkflowOid), Guid.Parse(request.TaskOid),
                request.User, request.Message);
        }

        /// <summary>
        /// Get Workflow Children
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowChildrenResponse</returns>
        public GetWorkflowChildrenResponse GetWorkflowChildren(GetWorkflowChildrenRequest request)
        {
            var oids = _workflow.GetWorkflowChildren(Guid.Parse(request.WorkflowOid));
            var ids = new List<string>();
            foreach (var oid in oids)
            {
                ids.Add(oid.ToString());
            }

            return new GetWorkflowChildrenResponse
            {
                Children = ids
            };
        }

        /// <summary>
        /// Cancel Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ControlWorkflowResponse</returns>
        public ControlWorkflowResponse CancelWorkflow(ControlWorkflowRequest request)
        {
            int tot;
            IEnumerable<string> workflowCodes;
            var oldWf = _workflow.GetWorkflows(Guid.Parse(request.WorkflowOid),
                string.Empty, string.Empty, false, null, null, string.Empty, string.Empty, 0, 10, out tot, out workflowCodes).First();

            var response = new ControlWorkflowResponse { Message = Library.Properties.Resources.SUCCESS_RESULT };

            if (oldWf.Status == WorkflowStatusType.InProgress.ToString())
            {
                try
                {
                    CancelWorkflowWithHisChildren(Guid.Parse(request.WorkflowOid));
                    response.Message = Library.Properties.Resources.SUCCESS_RESULT;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }

            return response;
        }

        /// <summary>
        /// Delete Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ControlWorkflowResponse</returns>
        public ControlWorkflowResponse DeleteWorkflow(ControlWorkflowRequest request)
        {
            int tot;
            IEnumerable<string> workflowCodes;
            var oldWf = _workflow.GetWorkflows(Guid.Parse(request.WorkflowOid),
                string.Empty, string.Empty, false, null, null, string.Empty, string.Empty, 0, 10, out tot, out workflowCodes).First();

            var response = new ControlWorkflowResponse { Message = Library.Properties.Resources.SUCCESS_RESULT };

            if (oldWf.Status == WorkflowStatusType.InProgress.ToString())
            {
                try
                {
                    CancelWorkflowWithHisChildren(Guid.Parse(request.WorkflowOid));
                    response.Message = Library.Properties.Resources.SUCCESS_RESULT;
                }
                catch
                {
                    // do nothing. delete it from database anyway
                }

            }

            _workflow.DeleteWorkflow(Guid.Parse(request.WorkflowOid));

            return response;
        }

        /// <summary>
        /// ReStart Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReStartWorkflowResponse</returns>
        public ReStartWorkflowResponse ReStartWorkflow(ReStartWorkflowRequest request)
        {
            // Cancell workflow
            var resp = CancelWorkflow(new ControlWorkflowRequest
            {
                WorkflowOid = request.OldWorkflowId
            });

            if (resp.Message != Library.Properties.Resources.SUCCESS_RESULT)
            {
                return new ReStartWorkflowResponse
                {
                    Message = resp.Message,
                    WorkflowId = request.OldWorkflowId
                };
            }

            // Wait for workflow to terminate
            WorkflowInfo oldWf = WaitForWorkflowTerminate(request);

            if (oldWf == null || oldWf.Status == WorkflowStatusType.InProgress.ToString())
            {
                return new ReStartWorkflowResponse
                {
                    Message = "Workflow still running",
                    WorkflowId = request.OldWorkflowId
                };
            }

            // Get workflow parameters
            var props = _workflow.GetWorkflowParameters(Guid.Parse(request.OldWorkflowId));

            var startWorkflowRequest = new StartWorkflowRequest
            {
                Domain = oldWf.Domain,
                WorkflowCode = oldWf.WorkflowCode,
                WfRuntimeValues = props.Select(p => new WfProperty
                {
                    Name = p.Name,
                    Type = p.Type,
                    Value = p.Value
                }).ToArray()
            };

            // Restart workflow
            var startWorkflowResponse = StartWorkflow(startWorkflowRequest);

#if RESTART_SAME_AS_OLD
            // The code below can be used when we want to restart a workflow and forward its
            // execution till the existing one.
            // Usefull when we make changes which are not back compatible. We need to terminate all
            // the existing workflows and start new ones without having users to start from the begin
            // the process.
            // TODO:
            // can go till the old point of execution
            if (request.RestartMode == "SAME_AS_OLD")
            {
                // TODO: need to find the way to wait until wf activity finishes.
                System.Threading.Thread.Sleep(3000);

                var traces = _workflow.GetTraceForWorkflow(new[] { Guid.Parse(request.OldWorkflowId) })
                    .Where(t => t.Action == ActionTrace.TaskCompleted.ToString());

                foreach (var trace in traces)
                {
                    var task = _task.GetNextTasksForWorkflow(Guid.Parse(startWorkflowResponse.WorkflowId))
                        .Where(t => t.TaskCode == trace.Code)
                        .FirstOrDefault();

                    if (task != null)
                    {
                        ApproveTask(new ApproveTaskRequest
                        {
                            TaskId = task.TaskOid.ToString(),
                            CorrelationId = task.TaskCorrelationId,
                            Result = trace.Result,
                            TaskCode = trace.Code,
                            WorkflowId = task.WorkflowOid.ToString()
                        });
                    }
                }
            }
#endif

            return new ReStartWorkflowResponse
            {
                WorkflowId = startWorkflowResponse.WorkflowId,
                Message = Library.Properties.Resources.SUCCESS_RESULT
            };
        }

        /// <summary>
        /// Complete Workflow
        /// </summary>
        /// <param name="request">Request</param>
        public void CompleteWorkflow(CompleteWorkflowRequest request)
        {
            _workflow.CompleteWorkflow(Guid.Parse(request.WorkflowId), request.WorkflowStatusType, request.Result, request.Message);
        }

        /// <summary>
        /// Create Workflow
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateWorkflow(CreateWorkflowRequest request)
        {
            _workflow.CreateWorkflow(Guid.Parse(request.WorkflowId), Guid.Parse(request.ParentWorkflowId),
                request.WorkflowCode, request.Domain, request.PropertyInfos);
        }

        /// <summary>
        /// Get Expiry TimeSpan
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowResultResponse</returns>
        public GetWorkflowResultResponse GetWorkflowResult(GetWorkflowResultRequest request)
        {
            return new GetWorkflowResultResponse
            {
                WorkflowResultInfo = _workflow.GetWorkflowResult(Guid.Parse(request.WorkflowId))
            };
        }

        /// <summary>
        /// Is Workflow In Progress
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>IsWorkflowInProgressResponse</returns>
        public IsWorkflowInProgressResponse IsWorkflowInProgress(IsWorkflowInProgressRequest request)
        {
            return new IsWorkflowInProgressResponse
            {
                IsInProgress = _workflow.IsWorkflowInProgress(Guid.Parse(request.WorkflowId))
            };

        }

        /// <summary>
        /// Create Topic
        /// </summary>
        /// <param name="request">Request</param>
        public CreateTopicResponse CreateTopic(CreateTopicRequest request)
        {
            return new CreateTopicResponse
                {
                    TopicId =
                        _messenger.CreateTopic(request.Title, request.Message, request.From, request.To,
                                               request.Attachments)
                };
        }

        /// <summary>
        /// Create Reply
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateReply(CreateReplyRequest request)
        {
            _messenger.CreateReply(request.TopicId, request.Message, request.From, request.To, request.Attachments);
        }

        /// <summary>
        /// Get Topics For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetTopicsForUserResponse GetTopicsForUser(GetTopicsForUserRequest request)
        {
            return new GetTopicsForUserResponse
            {
                Topics = new TopicInfos(_messenger.GetTopicsForUser(request.User, request.TopicId, request.Start, request.End, request.Title, request.Status, request.PageIndex, request.PageSize, request.WithReplies))
            };
        }

        /// <summary>
        /// Get Replies For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetRepliesForUserResponse GetRepliesForUser(GetRepliesForUserRequest request)
        {
            bool hasOldReplies;
            var resp = new TopicMessageInfos(_messenger.GetRepliesForUser(request.TopicId, request.User, request.Start, request.End,
                                                               request.ShowType, out hasOldReplies));

            return new GetRepliesForUserResponse
            {
                Replies = resp,
                HasOldReplies = hasOldReplies
            };
        }

        /// <summary>
        /// Get Topic Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetTopicCountResponse GetTopicCount(GetTopicCountRequest request)
        {
            return new GetTopicCountResponse
            {
                Count = _messenger.GetTopicCount(request.User)
            };
        }

        /// <summary>
        /// Search For Topics
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public SearchForTopicsResponse SearchForTopics(SearchForTopicsRequest request)
        {
            return new SearchForTopicsResponse
            {
                Result = new SearchInfos(_messenger.SearchForTopics(request.User, request.Pattern))
            };
        }

        /// <summary>
        /// Apply for Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public ApplyHolidayResponse ApplyHoliday(ApplyHolidayRequest request)
        {
            var h = _holiday.ApplyHoliday(request.User, request.Type, request.Holiday);

            return new ApplyHolidayResponse {HolidayId = h};
        }

        /// <summary>
        /// Get Holiday For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetHolidayForUserResponse GetHolidayForUser(GetHolidayForUserRequest request)
        {
            var resp = _holiday.GetHolidayForUser(request.User, request.HolidayId, request.Year);

            return new GetHolidayForUserResponse {Holidays = resp};
        }

        /// <summary>
        /// Update Holiday
        /// </summary>
        /// <param name="request"></param>
        public void UpdateHoliday(UpdateHolidayRequest request)
        {
            _holiday.UpdateHoliday(request.User, request.HolidayId, request.Status, request.Holiday);
        }

        /// <summary>
        /// Remove Holiday
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public RemoveHolidayResponse RemoveHoliday(RemoveHolidayRequest request)
        {
            var h = _holiday.RemoveHoliday(request.User, request.HolidayId);

            return new RemoveHolidayResponse { IsRemoved = h };
        }

        #endregion

        #region Tasks

        /// <summary>
        /// ApproveTask
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ApproveTaskResponse</returns>
        public ApproveTaskResponse ApproveTask(ApproveTaskRequest request)
        {
            ApproveTaskResponse response = null;

            if (_task.IsTaskNotification(Guid.Parse(request.TaskId)))
            {
                _task.CompleteNotification(Guid.Parse(request.TaskId));
                return new ApproveTaskResponse { TaskId = request.TaskId, WorkflowId = request.WorkflowId };
            }

            var proxy = _proxyManager.GetProxyForWorkflow(request.WorkflowId);

            switch (request.CorrelationId)
            {
                case 1: response = proxy.ApproveTask1(request);
                    break;
                case 2: response = proxy.ApproveTask2(request);
                    break;
                case 3: response = proxy.ApproveTask3(request);
                    break;
                case 4: response = proxy.ApproveTask4(request);
                    break;
                case 5: response = proxy.ApproveTask5(request);
                    break;
                default: throw new ArgumentException("Wrong CorrelationId must be 1-5");
            }

            var toDispose = proxy as System.ServiceModel.ClientBase<IFlowTasksOperations>;
            if (toDispose != null) toDispose.Close();

            return response;
        }

        /// <summary>
        /// Workflow Event
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowEventResponse</returns>
        public WorkflowEventResponse WorkflowEvent(WorkflowEventRequest request)
        {
            WorkflowEventResponse response = null;

            var proxy = _proxyManager.GetProxyForWorkflow(request.WorkflowId);

            switch (request.CorrelationId)
            {
                case 1: response = proxy.WorkflowEvent1(request);
                    break;
                case 2: response = proxy.WorkflowEvent2(request);
                    break;
                case 3: response = proxy.WorkflowEvent3(request);
                    break;
                case 4: response = proxy.WorkflowEvent4(request);
                    break;
                case 5: response = proxy.WorkflowEvent5(request);
                    break;
                default: throw new ArgumentException("Wrong CorrelationId must be 1-5");
            }

            var toDispose = proxy as System.ServiceModel.ClientBase<IFlowTasksOperations>;
            if (toDispose != null) toDispose.Close();

            return response;
        }

        /// <summary>
        /// Get Next Tasks For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetNextTasksForUserResponse</returns>
        public GetNextTasksForUserResponse GetNextTasksForUser(GetNextTasksForUserRequest request)
        {
            var response = new GetNextTasksForUserResponse();

            response.Tasks = _task.GetNextTasksForUser(request.User, request.WorkflowOid, request.Domain, request.PageIndex, request.PageSize, request.SearchFor).ToArray();

            return response;
        }

        /// <summary>
        /// Get Next Tasks For Workflow
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetNextTasksForWorkflowResponse</returns>
        public GetNextTasksForWorkflowResponse GetNextTasksForWorkflow(GetNextTasksForWorkflowRequest request)
        {
            var response = new GetNextTasksForWorkflowResponse();

            response.Tasks = _task.GetNextTasksForWorkflow(request.WorkflowOid).ToArray();

            return response;
        }

        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetTaskResponse</returns>
        public GetTaskResponse GetTask(GetTaskRequest request)
        {
            return new GetTaskResponse {Task = _task.GetTask(request.TaskOid)};
        }

        /// <summary>
        /// Give Back Task
        /// </summary>
        /// <param name="request">Request</param>
        public void GiveBackTask(GiveBackTaskRequest request)
        {
            _task.GiveBackTask(request.TaskOid);
        }

        /// <summary>
        /// Hand Over Task To
        /// </summary>
        /// <param name="request">Request</param>
        public void HandOverTaskTo(HandOverTaskToRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Users For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetUsersForTaskResponse</returns>
        public GetUsersForTaskResponse GetUsersForTask(GetUsersForTaskRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Hand Over Users For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetHandOverUsersForTaskResponse</returns>
        public GetHandOverUsersForTaskResponse GetHandOverUsersForTask(GetHandOverUsersForTaskRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Assign Task To
        /// </summary>
        /// <param name="request">Request</param>
        public void AssignTaskTo(AssignTaskToRequest request)
        {
            _task.AssignTaskTo(request.User, request.TaskOid);
        }

        /// <summary>
        /// Get Expiry Datetime
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetExpiryDatetimeResponse</returns>
        public GetExpiryDatetimeResponse GetExpiryDatetime(GetExpiryDatetimeRequest request)
        {
            return new GetExpiryDatetimeResponse { Expires = _task.GetExpiryDatetime(request.ExpiresWhen, request.ExpiresIn) };
        }

        /// <summary>
        /// Get Properties For Task
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetPropertiesForTaskResponse</returns>
        public GetPropertiesForTaskResponse GetPropertiesForTask(GetPropertiesForTaskRequest request)
        {
            return new GetPropertiesForTaskResponse { Properties = new PropertyInfos(_task.GetTaskParameters(request.TaskOid)) };
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateTask(CreateTaskRequest request)
        {
            _task.CreateTask(Guid.Parse(request.WorkflowId), request.TaskInfo, request.Properties);
        }

        /// <summary>
        /// Create Notification
        /// </summary>
        /// <param name="request">Request</param>
        public void CreateNotification(CreateNotificationRequest request)
        {
            _task.CreateNotification(Guid.Parse(request.WorkflowId), request.NotificationInfo);
        }

        /// <summary>
        /// Complete Task
        /// </summary>
        /// <param name="request">Request</param>
        public void CompleteTask(CompleteTaskRequest request)
        {
            _task.CompleteTask(Guid.Parse(request.TaskId), request.Result, request.User);
        }

        /// <summary>
        /// Get Expiry TimeSpan
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetExpiryTimeSpanResponse</returns>
        public GetExpiryTimeSpanResponse GetExpiryTimeSpan(GetExpiryTimeSpanRequest request)
        {
            return new GetExpiryTimeSpanResponse
            {
                Expires = _task.GetExpiryTimeSpan(request.ExpiresWhen, request.ExpiresIn)
            };
        }

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowTypeResponse</returns>
        public GetWorkflowTypeResponse GetWorkflowType(GetWorkflowTypeRequest request)
        {
            return new GetWorkflowTypeResponse
            {
                WorkflowTypeInfos = new WorkflowTypeInfos(_workflow.GetWorkflowType(request.EffectiveDate))
            };
        }

        /// <summary>
        /// Search Workflows
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>SearchWorkflowsResponse</returns>
        public SearchWorkflowsResponse SearchWorkflows(SearchWorkflowsRequest request)
        {
            return new SearchWorkflowsResponse
            {
                WorkflowInfos = new WorkflowInfos(_workflow.SearchWorkflows(request.WorkflowCode, request.Domain, request.Properties))
            };
        }

        /// <summary>
        /// Get Workflow Type
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetWorkflowResponse</returns>
        public GetWorkflowResponse GetWorkflows(GetWorkflowRequest request)
        {
            int tot;
            IEnumerable<string> workflowCodes;
            var workflows = _workflow.GetWorkflows(
                string.IsNullOrWhiteSpace(request.WorkflowId) ? Guid.Empty : Guid.Parse(request.WorkflowId),
                request.WorkflowCode, request.Domain, request.IsActive, request.StartedFrom, request.StartedTo,
                request.User, request.Role, request.PageIndex, request.PageSize, out tot, out workflowCodes);

            return new GetWorkflowResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalWorkflows = tot,
                WorkflowCodes = workflowCodes,
                WorkflowInfos = new WorkflowInfos(workflows)
            };
        }

        /// <summary>
        /// Report User Tasks
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportUserTasksResponse</returns>
        public ReportUserTasksResponse ReportUserTasks(ReportUserTasksRequest request)
        {
            return new ReportUserTasksResponse
            {
                Report = new ReportUserTasksInfos(_workflow.ReportUserTasks(request.Start, request.End, request.Users))
            };
        }

        /// <summary>
        /// Report Task Time
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportTaskTimeResponse</returns>
        public ReportTaskTimeResponse ReportTaskTime(ReportTaskTimeRequest request)
        {
            return new ReportTaskTimeResponse
            {
                report = new ReportTaskTimeInfos(_workflow.ReportTaskTime(request.Start, request.End, request.Tasks, request.Workflows))
            };
        }

        /// <summary>
        /// Report Workflow Time
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportWorkflowTimeResponse</returns>
        public ReportWorkflowTimeResponse ReportWorkflowTime(ReportWorkflowTimeRequest request)
        {
            return new ReportWorkflowTimeResponse
            {
                Report = new ReportWorkflowTimeInfos(_workflow.ReportWorkflowTime(request.Start, request.End, request.Workflows))
            };
        }

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportUserTaskCountResponse</returns>
        public ReportUserTaskCountResponse ReportUserTaskCount(ReportUserTaskCountRequest request)
        {
            return new ReportUserTaskCountResponse
            {
                Report = new ReportUserTaskCountInfos(_workflow.ReportUserTaskCount(request.Start, request.End, request.Users, request.Tasks))
            };
        }

        /// <summary>
        /// Report User Task Count
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>ReportUserTaskCountResponse</returns>
        public GetTaskCountResponse GetTaskCount(GetTaskCountRequest request)
        {
            return new GetTaskCountResponse
            {
                Count = _task.GetTaskCount(request.User)
            };
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Cancel Workflow With His Children
        /// </summary>
        /// <param name="parentid">Parentid</param>
        private void CancelWorkflowWithHisChildren(Guid parentid)
        {
            var oids = _workflow.GetWorkflowChildren(parentid);

            foreach (var w in oids)
            {
                CancelWorkflowWithHisChildren(w);
            }

            Cancel(parentid);
        }

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="oid">Oid</param>
        private void Cancel(Guid oid)
        {
            int tot;
            IEnumerable<string> workflowCodes;
            var oldWf = _workflow.GetWorkflows(oid,
                string.Empty, string.Empty, false, null, null, string.Empty, string.Empty, 0, 10, out tot, out workflowCodes).First();

            if (oldWf.Status == WorkflowStatusType.InProgress.ToString())
            {
                _proxyManager.GetProxyForWorkflow(oldWf.WorkflowCode).TerminateWorkflow1(new TerminateWorkflowRequest
                {
                    WorkflowId = oid.ToString(),
                    Message = "Workflow terminated by user"
                });
            }
        }

        /// <summary>
        /// WaitForWorkflowTerminate
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>WorkflowInfo</returns>
        private WorkflowInfo WaitForWorkflowTerminate(ReStartWorkflowRequest request)
        {
            const int retryNum = 10;
            const int delayRetryMsec = 1000;

            WorkflowInfo oldWf = null;

            for (int wait = 0; wait < retryNum; wait++)
            {
                int tot;
                IEnumerable<string> workflowCodes;
                oldWf = _workflow.GetWorkflows(Guid.Parse(request.OldWorkflowId),
                    string.Empty, string.Empty, false, null, null, string.Empty, string.Empty, 0, 10, out tot, out workflowCodes).First();

                if (oldWf.Status == WorkflowStatusType.InProgress.ToString())
                {
                    System.Threading.Thread.Sleep(delayRetryMsec);
                }
                else break;
            }
            return oldWf;
        }

        #endregion

    }
}
