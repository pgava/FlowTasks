using System.Diagnostics;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http;
using Flow.Library.Security;

namespace Flow.Tasks.Web.Controllers.Api
{
    /// <summary>
    /// FlowTasks Authorize Attribute
    /// </summary>
    public class FlowTasksApiAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// List of roles
        /// </summary>
        public string[] UserRoles { get; set; }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = HttpContext.Current.User as FlowTasksPrincipal;

                foreach (var r in UserRoles)
                {
                    Debug.Assert(user != null, "user != null");

                    if (user.IsInRole(r)) return true;
                    return false;
                }
            }

            return base.IsAuthorized(actionContext);
            
        }
    }
}
