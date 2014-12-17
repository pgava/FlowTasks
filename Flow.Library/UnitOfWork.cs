using System;
using Flow.Library.Interfaces;

namespace Flow.Library
{
    /// <summary>
    /// UnitOfWork
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// ObjectContext
        /// </summary>
        protected IObjectContext ObjectContext;

        public UnitOfWork(IObjectContext objectContext)
        {
            ObjectContext = objectContext;
        }

        public UnitOfWork()
        {
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ObjectContext != null)
                {
                    ObjectContext.Dispose();
                }
            }
        }

        #endregion

        #region IUnitOfWork Members

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            ObjectContext.SaveChanges();
        }

        #endregion
    }
}