using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Flow.Library.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> AsQueryable();

        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> Find(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);

        T Single(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);

        T First(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        
        T FirstOrDefault(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);

        void Delete(T entity);

        void Insert(T entity);

        void Update(T entity);

        IEnumerable<TS> ExecuteFunction<TS>(string query, params SqlStoreParameter[] parameters);
        //int ExecuteFunction(string query, params SqlParameter[] parameters);
        IEnumerable<TS> ExecuteStoreQuery<TS>(string query, params object[] parameters);
        int ExecuteStoreCommand(string query, params object[] parameters);

    }
}