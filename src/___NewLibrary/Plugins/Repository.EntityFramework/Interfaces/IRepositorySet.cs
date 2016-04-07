using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework.Interfaces
{
    public interface IRepositorySet<TEntity> 
        where TEntity : class
    {
        IQueryable<TEntity> Query { get; }

        void Add(TEntity entity) ;

        void Remove(TEntity entity);
    }
}
