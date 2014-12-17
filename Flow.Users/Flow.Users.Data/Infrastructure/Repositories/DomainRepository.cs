using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flow.Users.Data.Core.Interfaces;
using Flow.Users.Data.Core;
using Flow.Library.Interfaces;
using Flow.Library;

namespace Flow.Users.Data.Infrastructure.Repositories
{
    public class DomainRepository : Repository<Domain>, IDomainRepository
    {
        public DomainRepository(IObjectSetFactory objectSetFactory)
            : base(objectSetFactory)
        { }

        #region IDomainRepository Members

        public Domain GetDomainByName(string name)
        {
            return (from d in this.AsQueryable()
                    where d.Name == name
                    select d).FirstOrDefault();
        }

        #endregion
    }
}
