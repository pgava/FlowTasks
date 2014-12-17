using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using Flow.Tasks.Web.Models;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class UserFollowingController : BaseApiController
    {
        public UserFollowingController(IFlowUsersService usersService)
            : base(usersService)
        {
        }

        /// <summary>
        /// Add Following
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/follows/cgrant/gpeck
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="following"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddFollowing(string name, string following)
        {
            try
            {
                UsersService.AddFollowingUser(new AddFollowingUserRequest { Follower = name, Following = following });
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Remove Following
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/follows/cgrant/gpeck
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="following"></param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage RemoveFollowing(string name, string following)
        {
            try
            {
                UsersService.RemoveFollowingUser(new RemoveFollowingUserRequest() { Follower = name, Following = following });
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}