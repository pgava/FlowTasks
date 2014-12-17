using System;
using System.Diagnostics;
using System.Web.Mvc;
using Flow.Library.Security;

namespace Flow.Tasks.View
{
    /// <summary>
    /// FlowTasks Authorize Attribute
    /// </summary>
    public class FlowTasksAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// List of roles
        /// </summary>
        public string[] UserRoles { get; set; }

        /// <summary>
        /// OnAuthorization
        /// </summary>
        /// <param name="filterContext">FilterContext</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = filterContext.HttpContext.User as FlowTasksPrincipal;

                foreach (var r in UserRoles)
                {
                    Debug.Assert(user != null, "user != null");

                    if (user.IsInRole(r)) return;
                }
            }
            throw new Exception("User not authorized");
        }
    }
}
