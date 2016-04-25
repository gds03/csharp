using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace VirtualNote.Database.Include
{
    public static class IncludeClass
    {
        public static IQueryable<T> IncludeSet<T>(IQueryable<T> query, Expression<Func<T, Object>> selector) where T : class {
            return query.Include(selector);
        }
    }
}
