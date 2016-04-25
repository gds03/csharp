using System;
using System.Linq;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Database
{
    public interface IRepository : IDisposable
    {
        IQueryable<T> Query<T>() where T : class, IDomainObject;
        void Insert<T>(T obj)    where T : class, IDomainObject;
        void Delete<T>(T obj)    where T : class, IDomainObject;

        void SaveToDisk();
    }
}
