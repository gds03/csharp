using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Database.Types.Generic
{/// <summary>
    ///     Wraps IQueryable interface, to not expose all LINQ methods and do not allow another layer to call
    ///     directly those methods.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class QueryResult<TSource>
    {
        internal IQueryable<TSource> Query { get; private set; }

        public QueryResult(IQueryable<TSource> Q)
        {
            if (Q == null)
                throw new ArgumentNullException("queryable source cannot be null");

            this.Query = Q;
        }
    }
}
