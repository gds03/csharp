using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Types.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace CustomComponents.Repository.Interfaces
{
    public interface IRepository : IDisposable
    { 
        /// <summary>
        ///     Returns a QueryResult with all available queries for the specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        QueryResult<T> Query<T>() where T : class;


        /// <summary>
        ///     Returns a list of T objects that match the specific predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        ///     Inserts the specific e object in the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        IRepository Insert<T>(T e) where T : class;

        /// <summary>
        ///     Deletes the specific e object from the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        IRepository Delete<T>(T e) where T : class;
    }
}
