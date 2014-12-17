using Flow.Users.Contract;
using Flow.Users.Contract.Interface;
using Flow.Users.Contract.Message;
using Ninject;

namespace Flow.Users.Service
{
    public class FlowUsersService : IFlowUsersService
    {
        [Inject]
        public IFlowUsersOperations _operations { get; set; }

        public FlowUsersService(IFlowUsersOperations operations)
        {
            _operations = operations;
        }

        #region IFlowUsersService Members

        /// <summary>
        /// Get Users By Domains
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetUsersByDomainResponse</returns>
        public GetUsersByDomainResponse GetUsersByDomains(GetUsersByDomainRequest request)
        {
            return new GetUsersByDomainResponse { Users = _operations.GetUsersByDomains(request.Domains) };
        }

        /// <summary>
        /// Get Users By Roles
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetUsersByRolesResponse</returns>
        public GetUsersByRolesResponse GetUsersByRoles(GetUsersByRolesRequest request)
        {
            return new GetUsersByRolesResponse { Users = _operations.GetUsersByRoles(request.Roles) };
        }

        /// <summary>
        /// Is Valid User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>IsValidUserResponse</returns>
        public IsValidUserResponse IsValidUser(IsValidUserRequest request)
        {
            return new IsValidUserResponse { IsValid = _operations.IsValidUser(request.Domain, request.User) };
        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>AuthenticateUserResponse</returns>
        public AuthenticateUserResponse AuthenticateUser(AuthenticateUserRequest request)
        {
            return new AuthenticateUserResponse { User = request.User, IsAuthenticated = _operations.AuthenticateUser(request.User, request.Password) };
        }

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetDomainsForUserResponse</returns>
        public GetDomainsForUserResponse GetDomainsForUser(GetDomainsForUserRequest request)
        {
            return new GetDomainsForUserResponse { Domains = _operations.GetDomainsForUser(request.User) };
        }

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetRolesForUserResponse</returns>
        public GetRolesForUserResponse GetRolesForUser(GetRolesForUserRequest request)
        {
            return new GetRolesForUserResponse { Roles = _operations.GetRolesForUser(request.User, request.IsPrimaryRole) };
        }

        /// <summary>
        /// Get Domains and Roles For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetDomainRoleForUserResponse</returns>
        public GetDomainRoleForUserResponse GetDomainRoleForUser(GetDomainRoleForUserRequest request)
        {
            return new GetDomainRoleForUserResponse { DomainRole = _operations.GetDomainRoleForUser(request.User) };
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetUserResponse GetUser(GetUserRequest request)
        {
            return new GetUserResponse { User = _operations.GetUser(request.User) };
        }

        /// <summary>
        /// Get User Names
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetUserNamesResponse GetUserNames(GetUserNamesRequest request)
        {
            return new GetUserNamesResponse { UserNames = _operations.GetUserNames(request.NameToSearch) };
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <returns>Response</returns>
        public GetRolesResponse GetRoles(GetRolesRequest request)
        {
            return new GetRolesResponse { Roles = _operations.GetRoles(request.roleToSearch) };
        }

        /// <summary>
        /// Add Following User
        /// </summary>
        /// <param name="request"></param>
        public void AddFollowingUser(AddFollowingUserRequest request)
        {
            _operations.AddFollowingUser(request.Follower, request.Following);
        }

        /// <summary>
        /// Remove Following User
        /// </summary>
        /// <param name="request"></param>
        public void RemoveFollowingUser(RemoveFollowingUserRequest request)
        {
            _operations.RemoveFollowingUser(request.Follower, request.Following);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="request"></param>
        public void UpdateUser(UpdateUserRequest request)
        {
            _operations.UpdateUser(request.User);
        }
        #endregion
    }
}
