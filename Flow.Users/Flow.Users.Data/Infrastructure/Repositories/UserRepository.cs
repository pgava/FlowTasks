using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flow.Users.Data.Core.Interfaces;
using Flow.Users.Data.Core;

namespace Flow.Users.Data.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        Flow.Users.Data.Flow.Users.Data.ntities userDb = new Flow.Users.Data.Flow.Users.Data.ntities();

        #region IUserRepository Members

        public IEnumerable<User> GetUserByEmail(string email)
        {
            return (from u in userDb.Users
                    where u.Email.ToLower() == email.ToLower()
                    select u).ToList();
        }

        public User GetUserByName(string name)
        {
            var users = from u in userDb.Users
                        where u.Name == name
                        select u;

            return users.FirstOrDefault();
        }

        public User GetUserByCredentials(string userName, string password)
        {
            var users = from u in userDb.Users
                        where u.Name == userName && u.Password == password
                        select u;

            return users.FirstOrDefault();
        }

        public IEnumerable<User> GetUsersByRole(string role)
        {
            return (from du in userDb.DomainUsers
                   from r in du.Roles
                   where r.Name.ToLower() == role.ToLower()
                   orderby du.UserId
                   select du.User).ToList();
        }

        #endregion
    }
}
