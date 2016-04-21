using Repository.EntityFramework.Interfaces;
using Repository.EntityFramework.Types.ObjectContextRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Interfaces;
using System.Linq.Expressions;

namespace Repository.EntityFramework.Types.ObjectContextRepository
{
    public class ObjectContextRepository : ObjectContextRepositoryBase
    {
        public ObjectContextRepository(ObjectContext context) : base(context)
        {

        }

        public override IRepository Delete<TEntity>(TEntity @object)
        {
            this.Table<TEntity>().DeleteObject(@object);
            return this;
        }

        public override IRepository Insert<TEntity>(TEntity @object)
        {
            this.Table<TEntity>().AddObject(@object);
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

        private ObjectSet<TEntity> Table<TEntity>() where TEntity : class
        {
            return (ObjectSet < TEntity > ) 
                ObjectContext.GetType().GetProperty(typeof(TEntity).Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(ObjectContext, null);
        }
    }
}
