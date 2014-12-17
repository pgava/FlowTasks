using System.ServiceModel;
using Flow.Users.Contract.Message;

namespace Flow.Users.Contract
{
    /// <summary>
    /// FlowUsersService Interface
    /// </summary>
    [ServiceContract(Namespace = "http://flowtasks.com/")]
    public interface IFlowUsersService
    {
        /// <summary>
        /// Get Users By Domain
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetUsersByDomains", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetUsersByDomainResponse")]
        [return: MessageParameter(Name = "response")]
        GetUsersByDomainResponse GetUsersByDomains(GetUsersByDomainRequest request);

        /// <summary>
        /// Get Users By Roles
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetUsersByRoles", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetUsersByRolesResponse")]
        [return: MessageParameter(Name = "response")]
        GetUsersByRolesResponse GetUsersByRoles(GetUsersByRolesRequest request);

        /// <summary>
        /// Is Valid User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/IsValidUser", ReplyAction = "http://flowtasks.com/IFlowUsersService/IsValidUserResponse")]
        [return: MessageParameter(Name = "response")]
        IsValidUserResponse IsValidUser(IsValidUserRequest request);

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/AuthenticateUser", ReplyAction = "http://flowtasks.com/IFlowUsersService/AuthenticateUserResponse")]
        [return: MessageParameter(Name = "response")]
        AuthenticateUserResponse AuthenticateUser(AuthenticateUserRequest request);

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetDomainsForUser", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetDomainsForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetDomainsForUserResponse GetDomainsForUser(GetDomainsForUserRequest request);

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetRolesForUser", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetRolesForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetRolesForUserResponse GetRolesForUser(GetRolesForUserRequest request);

        /// <summary>
        /// Get Domains and Roles For User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetDomainRoleForUser", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetDomainRoleForUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetDomainRoleForUserResponse GetDomainRoleForUser(GetDomainRoleForUserRequest request);

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetUser", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetUserResponse")]
        [return: MessageParameter(Name = "response")]
        GetUserResponse GetUser(GetUserRequest request);

        /// <summary>
        /// Get User Names
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetUserNames", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetUserNamesResponse")]
        [return: MessageParameter(Name = "response")]
        GetUserNamesResponse GetUserNames(GetUserNamesRequest request);

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/GetRoles", ReplyAction = "http://flowtasks.com/IFlowUsersService/GetRolesResponse")]
        [return: MessageParameter(Name = "response")]
        GetRolesResponse GetRoles(GetRolesRequest request);

        /// <summary>
        /// Add Following User
        /// </summary>
        /// <param name="request"></param>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/AddFollowingUser")]
        void AddFollowingUser(AddFollowingUserRequest request);

        /// <summary>
        /// Remove Following User
        /// </summary>
        /// <param name="request"></param>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/RemoveFollowingUser")]
        void RemoveFollowingUser(RemoveFollowingUserRequest request);

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="request"></param>
        [OperationContract(Action = "http://flowtasks.com/IFlowUsersService/UpdateUser")]
        void UpdateUser(UpdateUserRequest request);
    }
}
