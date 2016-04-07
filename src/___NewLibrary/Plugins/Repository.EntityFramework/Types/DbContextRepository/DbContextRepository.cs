using Repository.EntityFramework.Interfaces;
using Repository.EntityFramework.Types.ObjectContextRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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

        }
        
        protected override IRepositorySet<TEntity> SetHook<TEntity>(object DbSetOrObjectSet)
        {
            DbSet<TEntity> property = (DbSet<TEntity>)DbSetOrObjectSet;
            return new DbContextSet<TEntity>(property);
        }



        private class DbContextSet<TEntity> : IRepositorySet<TEntity>
            where TEntity : class
        {
            private readonly DbSet<TEntity> m_set;

            public DbContextSet(DbSet<TEntity> Set)
            {
                if (Set == null)
                    throw new ArgumentNullException("Set");

                m_set = Set;
            }


            public IQueryable<TEntity> Query
            {
                get { return m_set; }
            }

            public void Add(TEntity entity)
            {
                m_set.Add(entity);
            }

            public void Remove(TEntity entity)
            {
                m_set.Remove(entity);
            }
        }
    }
}
