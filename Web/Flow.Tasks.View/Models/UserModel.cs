using Flow.Library.Security;
using System.Security.Principal;

namespace Flow.Tasks.View.Models
{
    public class UserModel
    {
        public static bool IsInRole(IPrincipal user, string role)
        {
            if (user.Identity.IsAuthenticated)
            {
                var myUser = user as FlowTasksPrincipal;

                return myUser != null && myUser.IsInRole(role);
            }

            return false;
        }
    }
}
