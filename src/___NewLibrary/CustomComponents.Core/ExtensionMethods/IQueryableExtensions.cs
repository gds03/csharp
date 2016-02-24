using CustomComponents.Core.Interfaces;
using CustomComponents.Core.Types;
using CustomComponents.Core.Types.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class IQueryableExtensions
    {

        /// <summary>
        ///     Distincts elements from keySelector filter.
        /// </summary>
        public static IQueryable<TKey> Distinct<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector)
        {
            return query.GroupBy(keySelector).Select(x => x.Key);
        }


        /// <summary>
        ///     Sorts the element of the sequence in specific order according to the key.
        /// </summary>
        public static IQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> key, bool ascending)
        {
            if (ascending)
                return query.OrderBy(key);

            return query.OrderByDescending(key);
        }


        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string columnName)
        {
            return query.OrderBy(true, columnName);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string columnName)
        {
            return query.OrderBy(false, columnName);
        }











        /// <summary>
        ///     ((((orderColumnDefault)))) order
        /// </summary>
        public static IEnumerable<TSource> GetFromPageNumItems<TSource, TKey>(this IQueryable<TSource> q,
            Expression<Func<TSource, TKey>> orderColumnDefault,

            int? page = null, int? takeItems = null, bool ascendingSort = true,
            bool orderColumnDefaultAscending = true
        )
        {
            return q._GetFromPageNumItems<TSource, TKey, TKey>(orderColumnDefault, orderColumnDefaultAscending, null, false, null, page, takeItems, ascendingSort, null);
        }

        /// <summary>
        ///     ((((userColumn)))) > (((orderColumnDefault)))
        /// </summary>
        public static IEnumerable<TSource> GetFromPageNumItems<TSource, TKey>(this IQueryable<TSource> q,
            Expression<Func<TSource, TKey>> orderColumnDefault,

            string userColumn = null,

            int? page = null, int? takeItems = null, bool ascendingSort = true,

            bool orderColumnDefaultAscending = true)
        {
            return q._GetFromPageNumItems<TSource, TKey, TKey>(orderColumnDefault, orderColumnDefaultAscending, null, false, userColumn, page, takeItems, ascendingSort, null);
        }


        /// <summary>
        ///     ((((complexKeyOrderBy)))) > (((userColumn))) > ((orderColumnOverrided)) > (orderColumnDefault)
        /// </summary>
        public static IEnumerable<TSource> GetFromPageNumItems<TSource, TKey, TKeyOverrided>(this IQueryable<TSource> q,

            Expression<Func<TSource, TKey>> orderColumnDefault,
            Expression<Func<TSource, TKeyOverrided>> orderColumnOverrided,
            string userColumn,

            int? page = null, int? takeItems = null, bool ascendingSort = true,
            IComplexKeyOrderBy<TSource> complexKeyOrderBy = null,

            bool orderColumnDefaultAscending = true,
            bool orderColumnOverridedAscending = true)
        {
            return q._GetFromPageNumItems<TSource, TKey, TKeyOverrided>(orderColumnDefault, orderColumnDefaultAscending, orderColumnOverrided, orderColumnOverridedAscending, userColumn, page, takeItems, ascendingSort, complexKeyOrderBy);
        }



        /// <summary>
        ///      ((((orderByDefault))))
        ///     and return the query count results in Count property and Items betweeen page and take items.
        /// </summary>
        public static TableInfo<TSource> ToTableInfo<TSource, TKey>(this IQueryable<TSource> query,

            Expression<Func<TSource, TKey>> orderByDefault,

            int? page = null, int? takeItems = null, bool ascendingSort = true,
            bool orderByDefaultAscending = true)
        {
            return new TableInfo<TSource>(
                query.Count(),              // count function of SQL Server and >>>> without includes!!
                query.GetFromPageNumItems<TSource, TKey>(orderByDefault, page, takeItems, ascendingSort, orderByDefaultAscending)
            );
        }

        /// <summary>
        ///     ((((userColumn)))) > (((orderByDefault)))
        ///     and return the query count results in Count property and Items betweeen page and take items.
        /// </summary>
        public static TableInfo<TSource> ToTableInfo<TSource, TKey>(this IQueryable<TSource> query,

            Expression<Func<TSource, TKey>> orderByDefault,

            string userColumn,

            int? page = null, int? takeItems = null, bool ascendingSort = true,
            bool orderByDefaultAscending = true
            )
        {
            return new TableInfo<TSource>(
                query.Count(),              // count function of SQL Server and >>>> without includes!!
                query.GetFromPageNumItems<TSource, TKey>(orderByDefault, userColumn, page, takeItems, ascendingSort, orderByDefaultAscending)
            );
        }




        #region Helpers


        private static MemberExpression BuildPropertyExpression<T>(string FullPath, ParameterExpression parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            // 
            // determinate if is a combined property -> e.g: Especialidade1.nome

            Type CurrentType = typeof(T);
            string[] allProperties = FullPath.Trim().Split('.');


            MemberExpression finalExpression = null;
            Expression node = parameter;      // for start, the first node is the parameter (required for do the tree connections)

            PropertyInfo currentProperty = null;

            for (int idx = 0; idx < allProperties.Length; idx++)
            {
                currentProperty = CurrentType.GetProperty(allProperties[idx], BindingFlags.Instance | BindingFlags.Public);

                if (currentProperty == null)
                    throw new InvalidOperationException("Property not found");

                MemberExpression propertyExpr = finalExpression = Expression.Property(node, currentProperty);     // 1) x => x.Especialidade, 2) x => x.Especialidade.nome

                bool isLastIndex = (idx == (allProperties.Length - 1));

                if (!isLastIndex)
                {
                    CurrentType = currentProperty.PropertyType;
                    node = propertyExpr;
                }
            }

            return finalExpression;
        }




        // columnName, can be a Property of some another property.
        private static IQueryable<T> OrderBy<T>(this IQueryable<T> query, bool ascending, string columnName)
        {
            // checks
            if (string.IsNullOrEmpty(columnName))
                throw new InvalidOperationException();

            if (query == null)
                throw new ArgumentNullException("query");

            //
            // Compose the expression tree that represents the parameter to the predicate.            

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");                    // x =>
            MemberExpression finalExpression = BuildPropertyExpression<T>(columnName, parameter);

            // add method call expression to root expression
            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                (ascending) ? "OrderBy" : "OrderByDescending",
                new[] { query.ElementType, finalExpression.Type },
                query.Expression,
                Expression.Lambda(finalExpression, new ParameterExpression[] { parameter })
            );

            return query.Provider.CreateQuery<T>(orderByCallExpression);
        }



        /// <summary>
        ///     If you do not pass page and takeItems, that assumes that you want to return all data.
        ///     If you pass page and takeItems, this will return only a portion of objects.
        ///     Order by order: ((((complexKeyOrderBy)))) > (((columnName))) > ((orderColumnOverrided)) > (orderColumnDefault)
        /// </summary>
        private static IEnumerable<TSource> _GetFromPageNumItems<TSource, TKeyDefault, TKeyOverrided>(this IQueryable<TSource> query,

            Expression<Func<TSource, TKeyDefault>> orderColumnDefault,
            bool orderColumnDefaultAscending,

            Expression<Func<TSource, TKeyOverrided>> orderColumnOverrided,
            bool orderByOverridedAscending,

            string columnName,
            int? page, int? takeItems, bool ascendingSort,

            IComplexKeyOrderBy<TSource> complexKeyOrderBy
        )
        {
            if (page == null && takeItems != null)
                throw new InvalidOperationException("For a null page value, takeItems must be null too");

            if (takeItems == null && page != null)
                throw new InvalidOperationException("For a null takeItems value, page must be null too");

            if (page <= 0)
                throw new InvalidOperationException("page cannot be less or equal than 0");

            if (takeItems <= 0)
                throw new InvalidOperationException("takeItems cannot be less or equal than 0");

            if (orderColumnDefault == null)
                throw new InvalidOperationException("You should set the default lambda expression to order by some column");

            if (page == null && takeItems == null)
            {
                //
                // Typically used for export operations..
                //
                // Do not use paginating method and return all.

                return query.OrderBy(orderColumnDefault, orderColumnDefaultAscending)
                            .ToList();
            }

            // Get the bucket
            int npage = page.Value;
            int ntakeItems = takeItems.Value;
            int bucket = (npage - 1) * ntakeItems;

            IQueryable<TSource> orderedResult = query.OrderByWhat<TSource, TKeyDefault, TKeyOverrided>(

                columnName, ascendingSort,
                orderColumnDefault, orderColumnDefaultAscending,
                orderColumnOverrided, orderByOverridedAscending,

                complexKeyOrderBy);

            // 
            // after order filter be applied, still limit the result set size.

            return orderedResult.Skip(bucket)
                                 .Take(ntakeItems)
                                 .ToList();
        }


        /// <summary>
        ///     Apply the orderby or orderbydescending filter over a query.
        ///     REMARKS: « orderByDefault « orderByOverrided « columnToOrder 
        ///     where columnToOrder takes high priority to order if defined and so on.
        /// </summary>
        private static IQueryable<TSource> OrderByWhat<TSource, TKey, TKeyOverrided>(this IQueryable<TSource> query,

            string columnToOrder,
            bool ascending,

            Expression<Func<TSource, TKey>> orderByDefault,
            bool orderColumnDefaultAscending,

            Expression<Func<TSource, TKeyOverrided>> orderByOverrided,
            bool orderByOverridedAscending,

            IComplexKeyOrderBy<TSource> complexKeyOrderBy
        )
        {
            if (orderByDefault == null)               // required
                throw new InvalidOperationException("You should set the default lambda expression to order by some column");

            if (complexKeyOrderBy != null)
            {
                IQueryable<TSource> orderedQuery = complexKeyOrderBy.ApplyOrder(query, columnToOrder, ascending);
                return orderedQuery;
            }

            //

            // column - string
            if (!string.IsNullOrEmpty(columnToOrder))
                return query.OrderBy(ascending, columnToOrder);     // builds lambda based on a string ( user input )

            // column - overrided
            if (orderByOverrided != null)
                return query.OrderBy(orderByOverrided, orderByOverridedAscending);             // LINQ order by 

            // column - default
            return query.OrderBy(orderByDefault, orderColumnDefaultAscending);
        }


        #endregion
    }
}
