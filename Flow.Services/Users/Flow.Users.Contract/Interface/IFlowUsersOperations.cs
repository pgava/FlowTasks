using Flow.Users.Contract.Message;
using System.Collections.Generic;

namespace Flow.Users.Contract.Interface
{
    public interface IFlowUsersOperations
    {
        /// <summary>
        /// Get Users By Domain
        /// </summary>
        /// <param name="domains">Domains</param>
        /// <returns>List of users</returns>
        IEnumerable<string> GetUsersByDomains(IEnumerable<string> domains);

        /// <summary>
        /// Get Users By Roles
        /// </summary>
        /// <param name="domain">Domain</param>
        /// <param name="roles">Roles</param>
        /// <returns></returns>
        IEnumerable<string> GetUsersByRoles(IEnumerable<string> roles);

        /// <summary>
        /// Check if a valid user for the domain
        /// </summary>
        /// <param name="domain">Domain</param>
        /// <param name="user">User</param>
        /// <returns>True if user belong to domain</returns>
        bool IsValidUser(string domain, string user);

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <returns>True if valid user and password match</returns>
        bool AuthenticateUser(string user, string password);

        /// <summary>
        /// Get Domains For User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>List of domains</returns>
        IEnumerable<string> GetDomainsForUser(string user);

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="isPrimaryRole"></param>
        /// <returns>List of roles</returns>
        IEnumerable<string> GetRolesForUser(string user, bool isPrimaryRole);

        /// <summary>
        /// Get Domains and Roles For User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>domains and roles</returns>
        DomainRoleInfo GetDomainRoleForUser(string user);

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>UserInfo</returns>
        UserInfo GetUser(string user);

        /// <summary>
        /// Get User Names
        /// </summary>
        /// <param name="nameToSearch">Name To Search</param>
        /// <returns>List of Users name</returns>
        IEnumerable<UserInfo> GetUserNames(string nameToSearch);

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="roleToSearch">Name To Search</param>
        /// <returns>List of Roles</returns>
        IEnumerable<RoleInfo> GetRoles(string roleToSearch);

        /// <summary>
        /// Add Following User
        /// </summary>
        /// <param name="follower">Follower</param>
        /// <param name="following">Following</param>
        void AddFollowingUser(string follower, string following);

        /// <summary>
        /// Remove Following User
        /// </summary>
        /// <param name="follower">follower</param>
        /// <param name="following">following</param>
        void RemoveFollowingUser(string follower, string following);

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user">User</param>
        void UpdateUser(UserInfo user);
    }
}
