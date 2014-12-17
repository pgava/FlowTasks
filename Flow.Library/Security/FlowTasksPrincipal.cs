using System.Linq;
using System.Security.Principal;

namespace Flow.Library.Security
{
    /// <summary>
    /// FlowTasksPrincipal
    /// </summary>
    public sealed class FlowTasksPrincipal : IPrincipal
    {
        /// <summary>
        /// Identity
        /// </summary>
        private readonly IIdentity _identity;

        public FlowTasksPrincipal(IIdentity identity)
        {
            _identity = identity;
        }

        #region IPrincipal Members

        /// <summary>
        /// Identity
        /// </summary>
        public IIdentity Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// Is In Role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>True or False</returns>
        public bool IsInRole(string role)
        {
            var id = _identity as FlowTasksIdentity;

            return id != null && id.AuthorizeData.Roles.Count(dr => dr == role.ToUpper()) > 0;
        }

        #endregion

    }
}
