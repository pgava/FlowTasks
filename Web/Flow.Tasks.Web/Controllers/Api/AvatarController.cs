using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class AvatarController : BaseApiController
    {
        public AvatarController(IFlowUsersService usersService)
            : base(usersService)
        {
        }

        /// <summary>
        /// Update Avatar
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/avatar/cgrant
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateAvatar(string name)
        {
            try
            {
                // TODO
                HttpPostedFileBase avatar = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);

                var fileName = Path.GetFileName(avatar.FileName);
                var fileUrl = "~/images/users/" + fileName;
                var filePath = HttpContext.Current.Server.MapPath(fileUrl);
                avatar.SaveAs(filePath);

                // this is not necessary
                // UsersService.UpdateUser(new UpdateUserRequest { User = new UserInfo{UserName = name, PhotoPath = fileUrl.Replace("~/", "")} });

                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StringContent(fileUrl);
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}