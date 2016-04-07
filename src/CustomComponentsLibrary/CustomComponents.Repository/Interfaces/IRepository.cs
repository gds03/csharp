using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Types.Generic;
using System;
using System.Data;

namespace CustomComponents.Repository.Interfaces
{
    public interface IRepository : IDisposable
    {
        /// <summary>
        ///     Allow Queries with LINQ to Entities throught IQueryable interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Teste</returns>
        QueryResult<T> Query<T>() where T : class;

        /// <summary>
        ///     Insert the e object in specific table.
        ///     The inserted object is only on database after Synchronize was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        IRepository Insert<T>(T e) where T : class;

        /// <summary>
        ///     Delete the e object from specific table.
        ///     The deleted object is only removed from database after Synchronize was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        IRepository Delete<T>(T e) where T : class;
    }
}
