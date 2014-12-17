using System;
using System.Net;
using System.Web.Http;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.View;
using Flow.Tasks.Web.Models;
using Flow.Users.Contract;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class RepliesController : BaseApiController
    {
        private DocUtils docUtils;

        public RepliesController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument, IFlowUsersService usersService)
            : base(tasksService, docsDocument, usersService)
        {
            docUtils = new DocUtils(docsDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/replies/1?user=cgrant&showType=All
        /// </remarks>
        /// <param name="tid"></param>
        /// <param name="user"></param>
        /// <param name="showType"></param>
        /// <returns></returns>
        [HttpGet]
        public TopicRepliesModel GetReplies(int tid, string user, string showType)
        {
            try
            {
                var response =
                    TasksService.GetRepliesForUser(new GetRepliesForUserRequest
                    {
                        TopicId = tid,
                        User = user,
                        Start = DateTime.Now,
                        End = DateTime.Now.AddMonths(-6),
                        ShowType = (RepliesShowType)Enum.Parse(typeof(RepliesShowType), showType)
                    });

                return new TopicRepliesModel(tid, response.Replies, response.HasOldReplies);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

    }
}