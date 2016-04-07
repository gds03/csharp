using Repository.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework.Types.ObjectContextRepository
{
    public class ObjectContextRepository : ObjectContextRepositoryBase
    {
        public ObjectContextRepository(ObjectContext context) : base(context)
        {

        }


        protected override IRepositorySet<TEntity> SetHook<TEntity>(object DbSetOrObjectSet)
        {
            ObjectSet<TEntity> property = (ObjectSet<TEntity>)DbSetOrObjectSet;
            return new ObjectContextSet<TEntity>(property);
        }



        private class ObjectContextSet<TEntity> : IRepositorySet<TEntity>
            where TEntity : class
        {
            private readonly ObjectSet<TEntity> m_set;

            public ObjectContextSet(ObjectSet<TEntity> Set)
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
                m_set.AddObject(entity);
            }

            public void Remove(TEntity entity)
            {
                m_set.DeleteObject(entity);
            }


        }
    }
}
