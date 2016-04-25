using System.Data.Entity;
using System.Linq;
using VirtualNote.Database.Configurations.Database;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Database
{
    public sealed class DiskRepository : IRepository
    {
        readonly DbContext _dbContext;

        public DiskRepository()
        {
            _dbContext = new VirtualNoteDbContext(this);
        }

        DbSet<T> Table<T>() where T : class
        {
            return _dbContext.Set<T>();
        }


        public IQueryable<T> Query<T>() where T : class, IDomainObject
        {
            return Table<T>();
        }

        public void Insert<T>(T obj) where T : class, IDomainObject
        {
            Table<T>().Add(obj);
        }

        public void Delete<T>(T obj) where T : class, IDomainObject
        {
            Table<T>().Remove(obj);
        }

        public void SaveToDisk()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
