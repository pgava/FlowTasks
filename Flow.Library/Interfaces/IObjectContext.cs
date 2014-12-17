using System;

namespace Flow.Library.Interfaces
{
    public interface IObjectContext : IDisposable
    {
        void SaveChanges();
    }
}