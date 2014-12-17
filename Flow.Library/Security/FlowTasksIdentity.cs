using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Flow.Library.Security
{
    /// <summary>
    /// FlowTasksIdentity
    /// </summary>
    public sealed class FlowTasksIdentity : IIdentity
    {
        /// <summary>
        /// User
        /// </summary>
        private readonly string _user = string.Empty;

        /// <summary>
        /// List of domains and roles user belong to
        /// </summary>
        private readonly AuthorizeData _authorizeData;

        /// <summary>
        /// Is Authenticated
        /// </summary>
        private readonly bool _isAuthenticated;

        public FlowTasksIdentity(string user, string userData)
        {
            _isAuthenticated = true;
            _user = user;

            GetUserData(userData, ref _user, ref _authorizeData);
        }

        /// <summary>
        /// Set User Data
        /// </summary>
        /// <remarks>
        /// UserData has this format: UserId=uuu|AuthData=ddd,rrr  ----> domain,role
        /// </remarks>
        /// <param name="user">User</param>
        /// <returns>UserData</returns>
        public static string SetUserData(string user)
        {
            return SetUserData(user, new AuthorizeData());
        }

        /// <summary>
        /// Set User Data
        /// </summary>
        /// <remarks>
        /// UserData has this format: UserId=uuu|AuthData=ddd,rrr  ----> domain,role
        /// </remarks>
        /// <param name="user">User</param>
        /// <param name="authodata">Data</param>
        /// <returns>UserData</returns>
        public static string SetUserData(string user, AuthorizeData authodata)
        {
            string userData = "UserId=" + user;
            userData += "|AuthData=" + authodata;

            return userData;
        }

        /// <summary>
        /// Get UserData
        /// </summary>
        /// <remarks>
        /// Split UserData in all its components
        /// </remarks>
        /// <param name="userData">UserData</param>
        /// <param name="user">User</param>
        /// <param name="authodata">Data</param>
        /// <returns>UserData</returns>
        public static string GetUserData(string userData, ref string user, ref AuthorizeData authodata)
        {
            foreach (var s in userData.Split(new[] { '|' }))
            {
                if (s.Contains("UserId="))
                {
                    user = s.Replace("UserId=", "");
                }
                else if (s.Contains("AuthData="))
                {
                    var dr = s.Replace("AuthData=", "").Trim();

                    authodata = new AuthorizeData(dr);
                }
            }

            return userData;
        }

        #region IIdentity Members

        /// <summary>
        /// Authentication Type
        /// </summary>
        public string AuthenticationType
        {
            get { return "FlowTasks"; }
        }

        /// <summary>
        /// Is Authenticated
        /// </summary>
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return _user; }
        }

        /// <summary>
        /// AuthorizeData
        /// </summary>
        public AuthorizeData AuthorizeData
        {
            get { return _authorizeData; }
        }

        #endregion
    }
}
