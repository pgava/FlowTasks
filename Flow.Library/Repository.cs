using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using Flow.Library.EF;
using Flow.Library.Interfaces;

namespace Flow.Library
{
    /// <summary>
    /// Repository
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// ObjectSet
        /// </summary>
        private readonly IObjectSet<T> _objectSet;

        /// <summary>
        /// ObjectSetFactory
        /// </summary>
        private readonly IObjectSetFactory _objectSetFactory;

        public Repository(IObjectSetFactory objectSetFactory)
        {
            _objectSet = objectSetFactory.CreateObjectSet<T>();
            _objectSetFactory = objectSetFactory;
        }

        #region IRepository<T> Members

        /// <summary>
        /// AsQueryable
        /// </summary>
        /// <returns>IQueryable list</returns>
        public IQueryable<T> AsQueryable()
        {
            return _objectSet;
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <param name="includeProperties">IncludeProperties</param>
        /// <returns>List of T</returns>
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = AsQueryable();
            query = PerformInclusions(includeProperties, query);
            return query;
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="includeProperties">IncludeProperties</param>
        /// <returns>Element of T</returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> where,
                                   params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = AsQueryable();
            query = PerformInclusions(includeProperties, query);
            return query.Where(where);
        }

        /// <summary>
        /// Single
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="includeProperties">IncludeProperties</param>
        /// <returns>Element of T</returns>
        public T Single(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = AsQueryable();
            query = PerformInclusions(includeProperties, query);
            return query.Single(where);
        }

        /// <summary>
        /// First
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="includeProperties">IncludeProperties</param>
        /// <returns>Element of T</returns>
        public T First(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = AsQueryable();
            query = PerformInclusions(includeProperties, query);
            return query.First(where);
        }

        /// <summary>
        /// First Or Default
        /// </summary>
        /// <param name="where">Where</param>
        /// <param name="includeProperties">IncludeProperties</param>
        /// <returns>Element of T</returns>
        public T FirstOrDefault(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = AsQueryable();
            query = PerformInclusions(includeProperties, query);
            return query.FirstOrDefault(where);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entity">Element of T</param>
        public void Delete(T entity)
        {
            _objectSet.DeleteObject(entity);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity">Element of T</param>
        public void Insert(T entity)
        {
            _objectSet.AddObject(entity);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity">Element of T</param>
        public void Update(T entity)
        {
            _objectSet.Attach(entity);
            _objectSetFactory.ChangeObjectState(entity, EntityState.Modified);
        }

        /// <summary>
        /// Execute Function
        /// </summary>
        /// <param name="query">Store proc or function</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>List of entity</returns>
        public IEnumerable<TS> ExecuteFunction<TS>(string query, params SqlStoreParameter[] parameters)
        {
            return _objectSetFactory.ExecuteFunction<TS>(query, parameters);
        }

        /// <summary>
        /// Execute Store Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>List of entity</returns>
        public IEnumerable<TS> ExecuteStoreQuery<TS>(string query, params object[] parameters)
        {
            return _objectSetFactory.ExecuteStoreQuery<TS>(query, parameters);
        }

        /// <summary>
        /// Execute Store Command
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Number of records</returns>
        public int ExecuteStoreCommand(string query, params object[] parameters)
        {
            return _objectSetFactory.ExecuteStoreCommand(query, parameters);
        }

        #endregion

        /// <summary>
        /// Perform Inclusions
        /// </summary>
        /// <param name="includeProperties">IncludeProperties</param>
        /// <param name="query">Query</param>
        /// <returns>IQueryable</returns>
        private static IQueryable<T> PerformInclusions(IEnumerable<Expression<Func<T, object>>> includeProperties,
                                                       IQueryable<T> query)
        {
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}