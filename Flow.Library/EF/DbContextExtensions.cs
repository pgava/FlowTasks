using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace Flow.Library.EF
{
    public static class DbContextExtensions
    {
        public static ObjectContext GetObjectContext(this DbContext dbContext)
        {
            return ((IObjectContextAdapter) dbContext).ObjectContext;
        }
    }
}