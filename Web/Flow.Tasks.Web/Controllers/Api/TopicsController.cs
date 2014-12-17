using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View;
using Flow.Tasks.Web.Models;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Flow.Tasks.Web.Controllers.Api
{
    public class TopicsController : BaseApiController
    {
        private DocUtils docUtils;

        public TopicsController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument, IFlowUsersService usersService)
            : base(tasksService, docsDocument, usersService)
        {
            docUtils = new DocUtils(docsDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/topics?user=cgrant&topicId=1
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpGet]
        public TopicsModel GetTopic(string user, int topicId)
        {
            try
            {
                var response = TasksService.GetTopicsForUser(new GetTopicsForUserRequest
                {
                    User = user,
                    TopicId = topicId,
                    Start = DateTime.Now,
                    End = DateTime.Now.AddMonths(-6),
                    Title = string.Empty,
                    Status = string.Empty,
                    PageIndex = 0,
                    PageSize = 1
                });
                return new TopicsModel(response.Topics);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/topics?user=cgrant&query=&status=&pageSize=5&pageIndex=1&winthReplies=false
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="query"></param>
        /// <param name="status"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="winthReplies"></param>
        /// <returns></returns>
        [HttpGet]
        public TopicsModel GetTopics(string user, string query, string status, int pageSize, int pageIndex, bool winthReplies=true)
        {
            try
            {

                var response = TasksService.GetTopicsForUser(new GetTopicsForUserRequest
                {
                    User = user,
                    Start = DateTime.Now,
                    End = DateTime.Now.AddMonths(-6),
                    Title = query,
                    Status = status,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    WithReplies = winthReplies
                });

                return new TopicsModel(response.Topics);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

        }

        [HttpGet]
        public object GetNewTopicsCount(string user)
        {
            return TasksService.GetTopicCount(new GetTopicCountRequest {User = user}).Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/topics?user=cgrant&searchFor=aaa
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="searchFor"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SearchResultModel> SearchTopicsFor(string user, string searchFor)
        {
            var topics = TasksService.SearchForTopics(new SearchForTopicsRequest {User = user, Pattern = searchFor});
            return topics.Result.Select(s => new SearchResultModel {Result = s.Message, Id = s.TopicId});
        }
        
        [HttpPost]
        public HttpResponseMessage AddTopic(string message, string from, IEnumerable<string> to, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var errorMessage = string.Empty;
                var attachments = docUtils.UploadFiles(from, files, ref errorMessage);

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Error uploading files: " + errorMessage);
                }

                TasksService.CreateTopic(new CreateTopicRequest
                    {
                        Message = message,
                        From = from,
                        To = String.Join(",", to),
                        Attachments = attachments
                    });

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddReply(string tid, string message, string from, IEnumerable<string> to, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var errorMessage = string.Empty;
                var attachments = docUtils.UploadFiles(from, files, ref errorMessage);

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error uploading files: " + errorMessage);
                }

                TasksService.CreateReply(new CreateReplyRequest
                {
                    From = from,
                    Message = message,
                    To = "",
                    TopicId = int.Parse(tid),
                    Attachments = attachments
                });

                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddReply(string tid, string from, string to)
        {
            try
            {
                var errorMessage = string.Empty;
                var message = HttpContext.Current.Request.Form["message"];
                IEnumerable<TopicAttachmentInfo> attachments = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    var files = Enumerable.Range(0, HttpContext.Current.Request.Files.Count).Select(
                        t =>
                        {
                            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[t]);
                            return filebase;
                        });

                    attachments = docUtils.UploadFiles(from, files, ref errorMessage);
                }
                
                var content = new AddReplyModel
                {
                    Result = true,
                    Reply = new CreateReplyRequest
                    {
                        From = from,
                        Message = message,
                        To = to,
                        TopicId = int.Parse(tid),
                        Attachments = attachments
                    }
                };
                
                //insert reply to topic
                TasksService.CreateReply(content.Reply);

                var json = JsonConvert.SerializeObject(
                            content,
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                          );
                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StringContent(json, Encoding.UTF8, "text/plain");
                return result;

            }
            catch (Exception e)
            {
                var content = new AddReplyModel { Result = false, ErrorMessage = e.Message };
                var json = JsonConvert.SerializeObject(
                            content,
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                          );
                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StringContent(json, Encoding.UTF8, "text/plain");
                return result;
            }
        }

        [HttpPost]
        public HttpResponseMessage AddTopic(string from, string to)
        {
            try
            {
                var toList = ((string.IsNullOrEmpty(to) ? "" : to) + ",").Split(',').Where(t => !string.IsNullOrEmpty(t)).ToList();
                var message = HttpContext.Current.Request.Form["message"];
                var errorMessage = string.Empty;
                IEnumerable<TopicAttachmentInfo> attachments = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    var files = Enumerable.Range(0, HttpContext.Current.Request.Files.Count).Select(
                        t =>
                        {
                            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[t]);
                            return filebase;
                        });

                    attachments = docUtils.UploadFiles(from, files, ref errorMessage);
                }

                var resp = TasksService.CreateTopic(new CreateTopicRequest
                {
                    Message = message,
                    From = from,
                    To = to,
                    Attachments = attachments
                });

                var fromUser = UsersService.GetUser(new GetUserRequest { User = from });
                if (fromUser.User == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                var content = new AddTopicModel
                {
                    Result = true,
                    TopicId = resp.TopicId,
                    Message = new TopicModelItem(new TopicMessageInfo
                    {
                        From = fromUser.User.UserName,
                        To = "",
                        ImageUrl = fromUser.User.PhotoPath,
                        When = DateTime.Now,
                        Message = message,
                        Attachments = attachments == null ? new List<TopicAttachmentInfo>() : attachments,
                        Status = TopicStatusType.New
                    }),
                    Replies = new List<TopicMessageInfo>().ToArray()
                };

                var result = Request.CreateResponse(HttpStatusCode.OK);
                var json = JsonConvert.SerializeObject(
                            content,
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                          );
                result.Content = new StringContent(json, Encoding.UTF8, "text/plain");
                return result;

            }
            catch (Exception e)
            {
                var content = new AddTopicModel { Result = false, ErrorMessage = e.Message };
                var json = JsonConvert.SerializeObject(
                            content,
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                          );
                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StringContent(json, Encoding.UTF8, "text/plain");
                return result;
            }
        }


        public class AddReplyModel
        {
            /// <summary>
            /// Gets or sets a value indicating whether the resut is true
            /// </summary>
            public bool Result { get; set; }
            /// <summary>
            /// Gets or sets the ErrorMessage
            /// </summary>
            public string ErrorMessage { get; set; }
            /// <summary>
            /// Gets or sets the Reply
            /// </summary>
            public CreateReplyRequest Reply { get; set; }
        }

        public class AddTopicModel
        {
            /// <summary>
            /// Gets or sets a value indicating whether the resut is true
            /// </summary>
            public bool Result { get; set; }
            /// <summary>
            /// Gets or sets the TopicId
            /// </summary>
            public int TopicId { get; set; }
            /// <summary>
            /// Gets or sets the ErrorMessage
            /// </summary>
            public string ErrorMessage { get; set; }
            /// <summary>
            /// Gets or sets the ErrorMessage
            /// </summary>
            public TopicModelItem Message { get; set; }
            /// <summary>
            /// Gets or sets the TopicMessageInfos
            /// </summary>
            public TopicMessageInfo[] Replies { get; set; }
        }

        public class SearchResultModel
        {
            public string Result { get; set; }
            public int Id { get; set; }
        }
    }

}