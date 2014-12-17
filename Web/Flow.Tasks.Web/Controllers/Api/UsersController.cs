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
    public class UsersController : BaseApiController
    {
        public UsersController(IFlowUsersService usersService)
            : base(usersService)
        {
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/cgrant
        /// </remarks>
        /// <param name="name">Name</param>
        /// <returns>User</returns>
        [HttpGet]
        public UserInfo GetUser(string name)
        {
            var resp = UsersService.GetUser(new GetUserRequest { User = name });
            if (resp.User == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return resp.User;
        }

        /// <summary>
        /// http://localhost/Flow.tasks.web/api/users?nameToSearch=cgr
        /// </summary>
        /// <param name="nameToSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<UserInfo> GetUserByName(string nameToSearch)
        {
            var users = UsersService.GetUserNames(new GetUserNamesRequest
            {
                NameToSearch = nameToSearch
            });

            return users.UserNames;
        }


        //http://localhost/Flow.tasks.web/api/users/
        [HttpPatch]
        public HttpResponseMessage UpdateUser(UserInfo user)
        {
            var c = System.Threading.Thread.CurrentThread.CurrentCulture;

            // convert date string to date object
            DateTime birthday;
            if (DateTime.TryParse(user.BirthdayStr, out birthday))
            {
                user.Birthday = birthday;
            }

            UsersService.UpdateUser(new UpdateUserRequest { User = user });
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Sign In
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Http Response</returns>
        [HttpPost]
        public HttpResponseMessage SignIn(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                var data = UsersService.AuthenticateUser(new AuthenticateUserRequest { User = model.UserName, Password = model.Password });
                if (data.IsAuthenticated)
                {
                    var response = Request.CreateResponse(HttpStatusCode.Created, true);
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return response;
                }
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "The user name or password provided is incorrect");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        /// <summary>
        /// Sign Out
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users
        /// </remarks>
        /// <returns>Http Response</returns>
        [HttpGet]
        public HttpResponseMessage SignOut()
        {
            var response = Request.CreateResponse(HttpStatusCode.Created, true);
            FormsAuthentication.SignOut();

            return response;
        }

    }
}