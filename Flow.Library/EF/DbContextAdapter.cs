using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Flow.Library.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Flow.Library.EF
{
    public abstract class DbContextAdapter : IObjectSetFactory
    {
        private readonly ObjectContext _context;

        abstract public string DatabaseName { get; }

        public DbContextAdapter(DbContext context)
        {
            _context = context.GetObjectContext();
        }

        #region IObjectContext Members

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #endregion

        #region IObjectSetFactory Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
            }
            
        }

        public IObjectSet<T> CreateObjectSet<T>() where T : class
        {
            return _context.CreateObjectSet<T>();
        }

        public void ChangeObjectState(object entity, EntityState state)
        {
            _context.ObjectStateManager.ChangeObjectState(entity, state);
        }

        public IQueryable<T> ExecuteFunction<T>(string query, params SqlStoreParameter[] parameters)
        {
            var objParams = new List<ObjectParameter>();

            foreach (var param in parameters)
            {
                objParams.Add(new ObjectParameter(param.Name, param.Value));
            }
            return _context.ExecuteFunction<T>(query, objParams.ToArray()).AsQueryable();
        }

        public int ExecuteFunction(string query, params SqlStoreParameter[] parameters)
        {
            var objParams = new List<ObjectParameter>();

            foreach (var param in parameters)
            {
                objParams.Add(new ObjectParameter(param.Name, param.Value));
            }
            return _context.ExecuteFunction(query, objParams.ToArray());
        }

        public IQueryable<T> ExecuteStoreQuery<T>(string query, params object[] parameters)
        {
            return _context.ExecuteStoreQuery<T>(query, parameters).AsQueryable();
        }

        public int ExecuteStoreCommand(string query, params object[] parameters)
        {
            return _context.ExecuteStoreCommand(query, parameters);
        }
        

        #endregion
    }
}