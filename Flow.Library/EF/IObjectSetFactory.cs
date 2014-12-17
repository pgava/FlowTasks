using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Flow.Library.Interfaces;

namespace Flow.Library.EF
{
    public interface IObjectSetFactory : IObjectContext
    {
        IObjectSet<T> CreateObjectSet<T>() where T : class;
        void ChangeObjectState(object entity, EntityState state);

        IQueryable<T> ExecuteFunction<T>(string query, params SqlStoreParameter[] parameters);
        int ExecuteFunction(string query, params SqlStoreParameter[] parameters);
        IQueryable<T> ExecuteStoreQuery<T>(string query, params object[] parameters);
        int ExecuteStoreCommand(string query, params object[] parameters);
    }
}