using System.ServiceModel;
using System.ServiceModel.Channels;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;

namespace Flow.Users.Proxy
{
    /// <summary>
    /// FlowUsersService
    /// </summary>
    public class FlowUsersService : ClientBase<IFlowUsersService>, IFlowUsersService
    {
        #region Ctors
        public FlowUsersService()
            : this("FlowUsersService_Endpoint")
        {
        }

        public FlowUsersService(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public FlowUsersService(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowUsersService(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public FlowUsersService(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        } 
        #endregion

        #region IFlowUsersService Members

        /// <summary>
        /// Get Users By Domains
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetUsersByDomainResponse</returns>
        public GetUsersByDomainResponse GetUsersByDomains(GetUsersByDomainRequest request)
        {
            return Channel.GetUsersByDomains(request);
        }

        /// <summary>
        /// Get Users By Roles
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetUsersByRolesResponse</returns>
        public GetUsersByRolesResponse GetUsersByRoles(GetUsersByRolesRequest request)
        {
            return Channel.GetUsersByRoles(request);
        }

        /// <summary>
        /// Is Valid User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>IsValidUserResponse</returns>
        public IsValidUserResponse IsValidUser(IsValidUserRequest request)
        {
            return Channel.IsValidUser(request);
        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>AuthenticateUserResponse</returns>
        public AuthenticateUserResponse AuthenticateUser(AuthenticateUserRequest request)
        {
            return Channel.AuthenticateUser(request);
        }

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetDomainsForUserResponse</returns>
        public GetDomainsForUserResponse GetDomainsForUser(GetDomainsForUserRequest request)
        {
            return Channel.GetDomainsForUser(request);
        }

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetRolesForUserResponse</returns>
        public GetRolesForUserResponse GetRolesForUser(GetRolesForUserRequest request)
        {
            return Channel.GetRolesForUser(request);
        }

        /// <summary>
        /// Get Domains and Roles For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>GetDomainRoleForUserResponse</returns>
        public GetDomainRoleForUserResponse GetDomainRoleForUser(GetDomainRoleForUserRequest request)
        {
            return Channel.GetDomainRoleForUser(request);
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetUserResponse GetUser(GetUserRequest request)
        {
            return Channel.GetUser(request);
        }

        /// <summary>
        /// Get User Names
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetUserNamesResponse GetUserNames(GetUserNamesRequest request)
        {
            return Channel.GetUserNames(request);
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        public GetRolesResponse GetRoles(GetRolesRequest request)
        {
            return Channel.GetRoles(request);
        }

        /// <summary>
        /// Add Following User
        /// </summary>
        /// <param name="request"></param>
        public void AddFollowingUser(AddFollowingUserRequest request)
        {
            Channel.AddFollowingUser(request);
        }

        /// <summary>
        /// Remove Following User
        /// </summary>
        /// <param name="request"></param>
        public void RemoveFollowingUser(RemoveFollowingUserRequest request)
        {
            Channel.RemoveFollowingUser(request);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="request"></param>
        public void UpdateUser(UpdateUserRequest request)
        {
            Channel.UpdateUser(request);
        }
        #endregion

    }
}
