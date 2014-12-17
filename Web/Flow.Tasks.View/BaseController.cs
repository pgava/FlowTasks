using System;
using System.Linq;
using System.Web.Mvc;
using Flow.Tasks.View.Configuration;
using System.IO;
using Flow.Tasks.Contract;
using Flow.Tasks.View.Models;
using Flow.Tasks.Contract.Message;
using Flow.Library;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Contract.Message;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;
using Flow.Library.Security;
using System.Web.Security;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;

namespace Flow.Tasks.View
{
    /// <summary>
    /// Base Controller
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// FlowUsersService
        /// </summary>
        protected IFlowUsersService UsersService;

        /// <summary>
        /// FlowTasksService
        /// </summary>
        protected IFlowTasksService TasksService;

        /// <summary>
        /// FlowDocsDocument
        /// </summary>
        protected IFlowDocsDocument DocsDocument;

        public BaseController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument)
        {
            UsersService = usersService;
            TasksService = tasksService;
            DocsDocument = docsDocument;
        }

        #region View

        /// <summary>
        /// Create View From Task Oid
        /// </summary>
        /// <param name="toid">Task OId</param>
        /// <returns>View</returns>
        protected ActionResult CreateViewFromTaskOid(string toid)
        {
            if (!string.IsNullOrWhiteSpace(toid))
            {
                var task = TasksService.GetTask(new GetTaskRequest { TaskOid = Guid.Parse(toid) });
                var docs = GetAttachedDocuments(task.Task.TaskOid);
                var comments = GetCommentsForTask(task.Task.WorkflowOid.ToString());
                var parameters = GetTaskParameters(task.Task.TaskOid);

                var model = CreateTask(task.Task, docs, comments, parameters);

                InitDocumentControl(model);

                InitCommentsControl(model);

                return View(model);
            }
            return View();
        }

        /// <summary>
        /// Create View From Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ActionResult CreateViewFromTaskOid(TaskModel model)
        {
            InitDocumentControl(model);

            InitCommentsControl(model);

            return View(model);
        }

        /// <summary>
        /// Create View From Form
        /// </summary>
        /// <param name="values">Form values</param>
        /// <param name="task">Task model</param>
        /// <returns>View</returns>
        protected ActionResult CreateViewFromForm(FormCollection values, TaskModel task)
        {
            InitApproveTask(values);

            InitDocumentControl(task);

            InitCommentsControl(task);

            return View(task);
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="t">Task</param>
        /// <param name="docs">Documents</param>
        /// <param name="coms">Comments</param>
        /// <param name="props">Properties</param>
        /// <returns>TaskModel</returns>
        protected TaskModel CreateTask(TaskInfo t, IEnumerable<DocumentModel> docs, IEnumerable<CommentItem> coms, IEnumerable<PropertyInfo> props)
        {
            var expires = t.ExpiryDate.HasValue ? t.ExpiryDate.Value.ToString("f") : "none";

            return new TaskModel
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
                Parameters = props.ToArray(),
                Filter = new FilterModel()
            };
        }

        #endregion View

        #region Document

        /// <summary>
        /// Init Document Control
        /// </summary>
        /// <param name="task">TaskModel</param>
        public virtual void InitDocumentControl(TaskModel task)
        {
            task.HasDocument = HasDocument(task.TaskOid);
        }

        /// <summary>
        /// Get Document Oid
        /// </summary>
        /// <param name="taskOid">TaskOid</param>
        /// <returns>Guid</returns>
        public string GetDocumentOid(string taskOid)
        {
            var oid = string.Empty;
            if (HasDocument(taskOid, ref oid))
            {
                return oid;
            }
            return Guid.Empty.ToString();
        }

        /// <summary>
        /// Has Document
        /// </summary>
        /// <param name="taskOid"></param>
        /// <param name="oid"></param>
        /// <returns>True or False</returns>
        private bool HasDocument(string taskOid, ref string oid)
        {
            var properties = TasksService.GetPropertiesForTask(
                new GetPropertiesForTaskRequest { TaskOid = Guid.Parse(taskOid) });

            var found = false;
            foreach (var prop in properties.Properties)
            {
                if (prop.Type == PropertyType.FlowDoc.ToString())
                {
                    oid = prop.Value;
                    found = true;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Has Document
        /// </summary>
        /// <param name="taskOid"></param>
        /// <returns>True or False</returns>
        private bool HasDocument(string taskOid)
        {
            var oid = string.Empty;
            return HasDocument(taskOid, ref oid);
        }

        /// <summary>
        /// Get Document
        /// </summary>
        /// <param name="oid">Oid docsDocument</param>
        /// <returns>ActionResult</returns>
        public ActionResult GetDocument(string oid)
        {
            var info = new DocumentInfo
            {
                OidDocument = Guid.Parse(oid),
                Version = 1
            };

            if (!string.IsNullOrWhiteSpace(oid))
            {
                DocsDocument.DownloadDocument(info, ConfigHelper.DownloadLocation, DocumentDownloadMode.LastVersion);
            }

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(info.DocumentName),

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(info.DocumentName, MimeType.GetMimeType(info.DocumentName));

        }

        /// <summary>
        /// Document
        /// </summary>
        /// <param name="values">FormCollection</param>
        /// <param name="task">TaskModel</param>
        /// <param name="documentOid"></param>
        /// <returns>ActionResult</returns>
        public ActionResult Document(FormCollection values, TaskModel task, string documentOid)
        {
            if (!string.IsNullOrWhiteSpace(documentOid))
            {
                return GetDocument(documentOid);
            }

            var properties = TasksService.GetPropertiesForTask(
                new GetPropertiesForTaskRequest { TaskOid = Guid.Parse(task.TaskOid) });

            var oid = string.Empty;
            foreach (var prop in properties.Properties)
            {
                if (prop.Type == PropertyType.FlowDoc.ToString())
                {
                    oid = prop.Value;
                    break;
                }
            }

            return GetDocument(oid);
        }

        /// <summary>
        /// Get Full Path
        /// </summary>
        /// <param name="workflowFile">WorkflowFile</param>
        /// <param name="serverPath">ServerPath</param>
        /// <returns>Full File Name</returns>
        public string GetFullPath(HttpPostedFileBase workflowFile, string serverPath)
        {
            if (workflowFile != null && workflowFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(workflowFile.FileName);
                Debug.Assert(fileName != null, "fileName != null");
                var filePath = Path.Combine(serverPath, fileName);

                return filePath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Copy Xmalx file to server
        /// </summary>
        /// <param name="workflowFile">WorkflowFile</param>
        /// <param name="serverPath">ServerPath</param>
        /// <param name="msg">msg</param>
        /// <returns>True or False</returns>
        public bool CopyFile(HttpPostedFileBase workflowFile, string serverPath, ref string msg)
        {
            try
            {
                if (workflowFile != null && workflowFile.ContentLength > 0)
                {
                    var filePath = GetFullPath(workflowFile, serverPath);

                    if (System.IO.File.Exists(filePath))
                    {
                        ArchiveExistingFile(filePath);
                    }
                    workflowFile.SaveAs(filePath);

                    return true;
                }
                msg = "Missing file or file empty";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return false;
        }

        /// <summary>
        /// Archive Existing File
        /// </summary>
        /// <param name="filePath">FilePath</param>
        private void ArchiveExistingFile(string filePath)
        {
            var archiveLocation = Path.Combine(ConfigHelper.DownloadLocation, "Archive");
            var currentFolder = Path.Combine(archiveLocation, DateTime.Now.ToString("yyyyMddHHmm"));

            // check for existence of folders
            if (!Directory.Exists(archiveLocation))
            {
                Directory.CreateDirectory(archiveLocation);
            }
            if (!Directory.Exists(currentFolder))
            {
                Directory.CreateDirectory(currentFolder);
            }

            if (string.IsNullOrWhiteSpace(filePath)) return;

            var fname = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fname)) return;

            var newFilePath = Path.Combine(currentFolder, fname);

            System.IO.File.Move(filePath, newFilePath);
        }

        /// <summary>
        /// Redirect From Area
        /// </summary>
        /// <param name="completeTask">CompleteTask</param>
        /// <param name="values">Values</param>
        /// <returns></returns>
        protected RedirectToRouteResult RedirectFromArea(string completeTask, FormCollection values)
        {
            TempData["CompleteTask"] = completeTask;
            TempData["FormValues"] = values;

            return RedirectToAction("Index", "TaskList", new { area = "", controller = "TaskList" });
        }

        #endregion

        #region Comments

        /// <summary>
        /// Init Comments Control
        /// </summary>
        /// <param name="task"></param>
        public virtual void InitCommentsControl(TaskModel task)
        {
            /*
             * TODO: Read from properties
             */
            task.Comment.Status = CommentModel.CommentStatus.Optional.ToString();
        }

        #endregion

        #region Authorization

        /// <summary>
        /// Authorize User To Domains
        /// </summary>
        /// <param name="data">AuthenticateUserResponse</param>
        /// <param name="role">Role</param>
        /// <param name="returnUrl">Return Url</param>
        /// <returns>ActionResult</returns>
        protected virtual ActionResult AuthorizeUserToDomains(AuthenticateUserResponse data, string role, string returnUrl, bool rememberMe = true)
        {
            string userData = String.Empty;

            if (data.IsAuthenticated)
            {
                var domainRoles = UsersService.GetDomainRoleForUser(new GetDomainRoleForUserRequest { User = data.User });

                userData = FlowTasksIdentity.SetUserData(data.User,
                    new AuthorizeData { Domains = domainRoles.DomainRole.Domanis, Roles = domainRoles.DomainRole.Roles });
            }

            AddAuthCookie(data.User, userData, rememberMe);

            // PG. Changes for New UI
            return Json(true);
            // END

            if (data.IsAuthenticated)
            {
                return RedirectToReturnUrl(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Authorize User To Domains
        /// </summary>
        /// <param name="data">AuthenticateUserResponse</param>
        /// <param name="returnUrl">Return Url</param>
        /// <returns>ActionResult</returns>
        protected virtual ActionResult AuthorizeUserToDomains(AuthenticateUserResponse data, string returnUrl, bool rememberMe)
        {
            return AuthorizeUserToDomains(data, string.Empty, returnUrl, rememberMe);
        }

        /// <summary>
        /// Add Authorize Cookie
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="userData">UserData</param>
        /// <param name="rememberMe"></param>
        protected virtual void AddAuthCookie(string name, string userData, bool rememberMe)
        {

            var expiry = DateTime.Now.AddMinutes(30);
            if (rememberMe)
            {
                expiry = DateTime.Now.AddDays(5);
            }
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, expiry, rememberMe, userData);
            //string encTicket = FormsAuthentication.Encrypt(ticket);
            //HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            HttpCookie faCookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, rememberMe);
            faCookie.Value = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(faCookie);
        }

        /// <summary>
        /// Redirect To Return Url
        /// </summary>
        /// <param name="returnUrl">Return Url</param>
        /// <returns>ActionResult</returns>
        private ActionResult RedirectToReturnUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Task

        /// <summary>
        /// Init Approve Task
        /// </summary>
        /// <param name="values">Values</param>
        protected void InitApproveTask(FormCollection values)
        {
            var request = new AssignTaskToRequest
            {
                User = HttpContext.User.Identity.Name,
                TaskOid = Guid.Parse(values["TaskOid"])
            };

            TasksService.AssignTaskTo(request);
        }

        /// <summary>
        /// Create TaskInfo From Database
        /// </summary>
        /// <param name="taskCode">TaskCode</param>
        /// <param name="acceptedBy">AcceptedBy</param>
        /// <param name="properties">Properties</param>
        /// <returns>TaskInfo</returns>
        protected TaskInfo CreateTaskInfoFromDatabase(string taskCode, string acceptedBy, IEnumerable<PropertyInfo> properties)
        {
            var task = TasksService.SearchForTasks(new SearchForTasksRequest
            {
                AcceptedBy = acceptedBy,
                TaskCode = taskCode,
                Properties = new PropertyInfos(properties)
            });

            if (task.Tasks != null && task.Tasks.Count() == 1)
            {
                return task.Tasks[0];
            }

            return null;
        }

        /// <summary>
        /// Create TaskModel From Database
        /// </summary>
        /// <param name="taskCode">TaskCode</param>
        /// <param name="acceptedBy">AcceptedBy</param>
        /// <param name="properties">Properties</param>
        /// <returns>TaskModel</returns>
        protected TaskModel CreateTaskModelFromDatabase(string taskCode, string acceptedBy, IEnumerable<PropertyInfo> properties)
        {
            var taskInfo = CreateTaskInfoFromDatabase(taskCode, acceptedBy, properties);
            TaskModel model = null;
            if (taskInfo != null)
            {
                var filterModel = GetFilterValues(model);
                var filter = new TaskFilter(filterModel);

                model = new TaskModel
                {
                    Description = taskInfo.Description,
                    DefaultResult = taskInfo.DefaultResult,
                    TaskCode = taskInfo.TaskCode,
                    TaskCorrelationId = taskInfo.TaskCorrelationId,
                    TaskOid = taskInfo.TaskOid.ToString(),
                    Title = taskInfo.Title,
                    UiCode = taskInfo.UiCode,
                    WorkflowOid = taskInfo.WorkflowOid.ToString(),
                    Filter = filter.GetFilterModel(),
                    Comment = new CommentModel
                    {
                        Comments = GetCommentsForTask(taskInfo.WorkflowOid.ToString()),
                        Status = CommentModel.CommentStatus.Optional.ToString(),
                        TaskComment = string.Empty
                    },
                    Parameters = GetTaskParameters(taskInfo.TaskOid),
                    Documents = GetAttachedDocuments(taskInfo.TaskOid)
                };
            }

            return model;
        }

        /// <summary>
        /// Get Filter Values
        /// </summary>
        /// <param name="task">TaskModel</param>
        /// <returns>FilterModel</returns>
        protected FilterModel GetFilterValues(TaskModel task)
        {
            if (task != null && task.Filter != null)
            {
                return task.Filter;
            }

            return new FilterModel
            {
                DisplayMethod = DisplayBy.All,
                Filter = string.Empty,
                OrderMethod = OrderListBy.TaskName,
                Domain = Domains.All,
                DomainList = GetDomainsForUser(),
                MaxTasks = MaxTasks.Tasks10
            };
        }

        /// <summary>
        /// Get Comments For Task
        /// </summary>
        /// <param name="workflowOid">Workflow Oid</param>
        /// <returns>List of CommentItem</returns>
        protected IEnumerable<CommentItem> GetCommentsForTask(string workflowOid)
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
        /// <param name="guid">Guid</param>
        /// <returns>List of PropertyInfo</returns>
        protected PropertyInfo[] GetTaskParameters(Guid guid)
        {
            var properties = TasksService.GetPropertiesForTask(new GetPropertiesForTaskRequest
            {
                TaskOid = guid
            });

            return properties.Properties.ToArray();
        }

        /// <summary>
        /// Get Attached Documents
        /// </summary>
        /// <param name="taskOid">TaskOid</param>
        /// <returns>List of v</returns>
        protected DocumentModel[] GetAttachedDocuments(Guid taskOid)
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
            }).ToArray();
        }

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <returns>List of domains</returns>
        protected IEnumerable<string> GetDomainsForUser()
        {
            var domains = UsersService.GetDomainsForUser(new GetDomainsForUserRequest { User = User.Identity.Name });

            return domains.Domains.Select(d => d);
        }

        #endregion
    }
}