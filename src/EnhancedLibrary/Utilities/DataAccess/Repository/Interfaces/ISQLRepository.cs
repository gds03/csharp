using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.Common;

namespace EnhancedLibrary.Utilities.DataAccess.Repository.Interfaces
{
    public interface ISQLRepository : IDisposable
    {
        // Get
        T QuerySingle<T>(Expression<Func<T, bool>> filter) where T : class;
        IEnumerable<T> Query<T>(Expression<Func<T, bool>> filter) where T : class;
        IEnumerable<T> Query<T>() where T : class;

        // Set
        void Insert<T>(T e) where T : class;
        void Update<T>(T e) where T : class;
        void Delete<T>(T e) where T : class;

        DbConnection Connection { get; }
    }
}
