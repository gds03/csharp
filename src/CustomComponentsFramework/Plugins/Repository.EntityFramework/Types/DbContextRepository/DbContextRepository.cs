using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Interfaces;
using Repository.EntityFramework.Interfaces;
using Repository.EntityFramework.Types.ObjectContextRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework.Types.DbContextRepository
{
    public class DbContextRepository : ObjectContextRepositoryBase
    {
        public DbContext DbContext { get; private set; }

        public DbContextRepository(DbContext context) : base(
            ((IObjectContextAdapter)context).ObjectContext
            )
        {
            if (context == null)
                throw new ArgumentNullException("context");

            DbContext = context;
        }



        public override IRepository Delete<TEntity>(TEntity @object)
        {
            this.Table<TEntity>().Remove(@object);
            return this;
        }

        public override IRepository Insert<TEntity>(TEntity @object)
        {
            this.Table<TEntity>().Add(@object);
            return this;
        }

        public override QueryResult<TEntity> Query<TEntity>()
        {
            return new QueryResult<TEntity>(Table<TEntity>());
        }

        public override IList<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return Table<TEntity>().Where(predicate).ToList();
        }

        private DbSet<TEntity> Table<TEntity>() where TEntity : class
        {
            return (DbSet<TEntity>)
                DbContext.GetType().GetProperty(typeof(TEntity).Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(DbContext, null);
        }
    }
}
