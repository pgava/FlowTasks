using System;
using System.Collections.Generic;

namespace Flow.Library.Security
{
    /// <summary>
    /// Authorize Data
    /// </summary>
    public class AuthorizeData
    {
        public IEnumerable<string> Domains { get; set; }
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="domainrole">comma separated</param>
        public AuthorizeData(string domainrole)
        {
            var dr = domainrole.ToUpper().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (dr.Length == 2)
            {
                Domains = dr[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Roles = dr[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                Domains = new List<string>();
                Roles = new List<string>();
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="domain">Domain</param>
        /// <param name="role">Role</param>
        public AuthorizeData(IEnumerable<string> domains, IEnumerable<string> roles)
        {
            Domains = domains;
            Roles = roles;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public AuthorizeData()
        {
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>To String</returns>
        public override string ToString()
        {
            var domains = string.Join(",", Domains);
            var roles = string.Join(",", Roles);

            return string.Format("{0};{1}", domains, roles);
        }
    }
}
