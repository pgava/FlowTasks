using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flow.Users.Data.Core.Interfaces;
using Flow.Users.Data.Core;

namespace Flow.Users.Data.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        Flow.Users.Data.Flow.Users.Data.ntities userDb = new Flow.Users.Data.Flow.Users.Data.ntities();


        #region IRoleRepository Members

        public Role GetRoleByName(string name)
        {
            return (from r in userDb.Roles
                    where r.Name == name
                    select r).FirstOrDefault();
        }

        #endregion
    }
}
