using System;
using System.Linq;
using System.Data.Objects.DataClasses;
using System.Data;

namespace EnhancedLibrary.ExternalTypes.ObjectContextAutoHistory
{
    public interface IRepositoryPattern : IDisposable
    {
        /// <summary>
        ///     Allow Queries with LINQ to Entities throught IQueryable interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Teste</returns>
        IQueryable<T> Query<T>() where T : EntityObject;

        /// <summary>
        ///     Insert the e object in specific table.
        ///     The inserted object is only on database after Synchronize was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        void Insert<T>(T e) where T : EntityObject;

        /// <summary>
        ///     Delete the e object from specific table.
        ///     The deleted object is only removed from database after Synchronize was called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        void Delete<T>(T e) where T : EntityObject;

        /// <summary>
        ///     Synchronize the database with all pending operations.
        /// </summary>
        void Synchronize();

        /// <summary>
        ///     Free all managed resources such the connection and ObjectContext associated with the repository
        /// </summary>
        void Dispose();


        /// <summary>
        ///     Get the connection to the repository
        /// </summary>
        IDbConnection RepositoryConnection { get; }
    }
}
