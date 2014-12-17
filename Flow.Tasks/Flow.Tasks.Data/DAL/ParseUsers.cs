using System;
using System.Linq;
using System.Collections.Generic;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;
using Flow.Users.Proxy;

namespace Flow.Tasks.Data.DAL
{
    internal class ParseUsers
    {
        /// <summary>
        /// Get List Users Name
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="domain">Domain</param>
        /// <param name="input">Input</param>
        /// <returns>List of string</returns>
        public static IEnumerable<string> GetListUsersName(IFlowUsersService usersService, string domain, string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new List<string>();

            var userSplit = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var userList = new List<string>();

            foreach (var u in userSplit)
            {
                AddUsersToList(GetUsersFromPlaceHolder(usersService, domain, u), userList);
            }

            return userList.Distinct();
        }

        /// <summary>
        /// Get List Users Name
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="input">Input</param>
        /// <returns>List of string</returns>
        public static IEnumerable<string> GetListUsersName(IFlowUsersService usersService, string input)
        {
            return GetListUsersName(usersService, string.Empty, input);
        }

        /// <summary>
        /// Add Users To List
        /// </summary>
        /// <param name="usersToAdd">Users To Add</param>
        /// <param name="userList">User List</param>
        private static void AddUsersToList(IEnumerable<string> usersToAdd, List<string> userList)
        {
            foreach (var u in usersToAdd)
            {
                if (string.IsNullOrWhiteSpace(u)) continue;
                if (userList.Any(s => s.Equals(u, StringComparison.OrdinalIgnoreCase))) continue;

                userList.Add(u);
            }
        }

        /// <summary>
        /// Get User From Name
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="domain">Domain</param>
        /// <param name="u">User</param>
        /// <returns>User</returns>
        private static string GetUserFromName(IFlowUsersService usersService, string domain, string u)
        {
            if (usersService == null)
            {
                using (var usersOperations = new FlowUsersService())
                {
                    return usersOperations.IsValidUser(new IsValidUserRequest { Domain = domain, User = u }).IsValid ? u : string.Empty;
                }
            }
            return usersService.IsValidUser(new IsValidUserRequest { Domain = domain, User = u }).IsValid ? u : string.Empty;
        }

        /// <summary>
        /// Get User From Role
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="r">User</param>
        /// <returns>User</returns>
        private static IEnumerable<string> GetUserFromRole(IFlowUsersService usersService, string r)
        {
            IEnumerable<string> users;

            if (usersService == null)
            {
                using (var usersOperations = new FlowUsersService())
                {
                    users = usersOperations.GetUsersByRoles(new GetUsersByRolesRequest { Roles = new[] { r } }).Users;
                    return users;
                }
            }

            users = usersService.GetUsersByRoles(new GetUsersByRolesRequest { Roles = new[] { r } }).Users;
            return users;
        }

        /// <summary>
        /// Get Users From Place Holder
        /// </summary>
        /// <param name="usersService"></param>
        /// <param name="domain">Domain</param>
        /// <param name="input"></param>
        /// <returns>List of string</returns>
        private static IEnumerable<string> GetUsersFromPlaceHolder(IFlowUsersService usersService, string domain, string input)
        {
            var userList = new List<string>();

            string user;
            string role;
            PlaceHolder.GetUserRoleFromPlaceHolder(input, out user, out role);

            if (!string.IsNullOrWhiteSpace(user))
            {
                var u = GetUserFromName(usersService, domain, user);
                if (!string.IsNullOrWhiteSpace(u)) userList.Add(u);
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var users = GetUserFromRole(usersService, role);
                var enumerable = users as string[] ?? users.ToArray();
                if (enumerable.Any()) userList.AddRange(enumerable);
            }

            return userList;
        }
    }
}
