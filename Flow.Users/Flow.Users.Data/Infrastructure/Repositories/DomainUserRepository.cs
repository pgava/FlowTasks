using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flow.Users.Data.Core.Interfaces;
using Flow.Users.Data.Core;
using Flow.Library.Interfaces;
using Flow.Library;
using System.Linq.Expressions;

namespace Flow.Users.Data.Infrastructure.Repositories
{
    public class DomainUserRepository : Repository<DomainUser>, IDomainUserRepository
    {
        public DomainUserRepository(IObjectSetFactory objectSetFactory)
            : base(objectSetFactory)
        { }

        Expression<Func<DomainUser, object>>[] includeProperties =
                new Expression<Func<DomainUser, object>>[] { d => d.Domain, u => u.User };

        #region IDomainUserRepository Members

        public IEnumerable<DomainUser> GetDomainUserByUser(string name)
        {
            return (from du in this.AsQueryable() // includeProperties
                    where du.User.Name == name
                    select du).ToList();
        }

        public IEnumerable<DomainUser> GetDomainUserByDomain(string name)
        {
            return (from du in this.AsQueryable() // includeProperties
                    where du.Domain.Name == name
                    select du).ToList();
        }

        public DomainUser GetDomainUserByDomainUser(string domain, string user)
        {
            return (from du in this.AsQueryable() // includeProperties
                    where du.Domain.Name == domain && du.User.Name == user
                    select du).FirstOrDefault();
        }

        #endregion
    }
}
