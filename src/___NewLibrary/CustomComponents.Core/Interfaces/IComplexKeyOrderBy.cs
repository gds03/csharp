using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Interfaces
{
    public interface IComplexKeyOrderBy<TSource>
    {
        IQueryable<TSource> ApplyOrder(IQueryable<TSource> source, string columnName, bool orderByAscending);
    }

    public interface IComplexKeyOrderBy
    {

    }
}
