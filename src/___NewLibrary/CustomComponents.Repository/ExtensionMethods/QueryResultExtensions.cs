using CustomComponents.Database.Types.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CustomComponents.Core.ExtensionMethods;

namespace CustomComponents.Database.ExtensionMethods
{
    public static class QueryResultExtensions
    {

        /// <summary>
        ///     Return all items in T table limited by MAX_ITEMS_TO_RETURN internal constant
        /// </summary>
        public static IEnumerable<TSource> GetAll<TSource>(this QueryResult<TSource> table)
        {
            return table.Query.ToList();
        }



        /// <summary>
        ///     Return all items in T table limited by MAX_ITEMS_TO_RETURN internal constant and ordered by orderColumnDefault lambda
        /// </summary>
        public static IEnumerable<TSource> GetAll<TSource, TKey>(this QueryResult<TSource> table, Expression<Func<TSource, TKey>> orderColumnDefault)
        {
            return table.Query.OrderBy(orderColumnDefault)
                              .ToList();
        }


        /// <summary>
        ///     ((((orderColumnDefault))))
        /// </summary>
        internal static IEnumerable<TSource> GetFromPageNumItems<TSource, TKey>(this QueryResult<TSource> table,
            Expression<Func<TSource, TKey>> orderColumnDefault, int? page = null, int? takeItems = null, bool ascendingSort = true,
            bool orderColumnDefaultAscending = true
            )
        {
            return table.Query.GetFromPageNumItems<TSource, TKey>(orderColumnDefault, page, takeItems, ascendingSort, orderColumnDefaultAscending);

        }

        /// <summary>
        ///     ((((userColumn)))) > (((orderColumnDefault)))
        /// </summary>
        internal static IEnumerable<TSource> GetFromPageNumItems<TSource, TKey>(this QueryResult<TSource> table,
            Expression<Func<TSource, TKey>> orderColumnDefault, string userColumn, int? page = null, int? takeItems = null, bool ascendingSort = true,
            bool orderColumnDefaultAscending = true
            )
        {
            return table.Query.GetFromPageNumItems<TSource, TKey>(orderColumnDefault, userColumn, page, takeItems, ascendingSort, orderColumnDefaultAscending);
        }
    }
}
