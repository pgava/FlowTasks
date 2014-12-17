using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class PasswordController : BaseApiController
    {
        public PasswordController(IFlowUsersService usersService)
            : base(usersService)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/password/cgrant/old/new
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="oldp"></param>
        /// <param name="newp"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdatePassword(string name, string oldp, string newp)
        {
            try
            {
                var data = UsersService.AuthenticateUser(new AuthenticateUserRequest { User = name, Password = oldp });
                if (data.IsAuthenticated)
                {
                    UsersService.UpdateUser(new UpdateUserRequest { User = new UserInfo { UserName = name, Password = newp } });
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Authentication failed");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

    }
}