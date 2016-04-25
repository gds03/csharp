using System;
using System.Linq;
using System.Linq.Expressions;
using VirtualNote.Database.Include;

namespace VirtualNote.Kernel.Query.Include
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Expression<Func<T, object>> selector) where T : class
        {
            return IncludeClass.IncludeSet(query, selector);
        }
    }
}
