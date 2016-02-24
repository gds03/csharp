using CustomComponents.Database.Types.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        ///     Synchronize the database entities with all pending operations in memory.
        /// </summary>
        IRepository Synchronize();


        /// <summary>
        ///     Execute user extern method within a transaction of a repository
        /// </summary>
        /// <param name="externMethod"></param>
        void ExecuteBlock(Action<IRepository> externMethod, Action<Exception> exceptionMethod = null);


        /// <summary>
        ///     Free the developer to use Using statement
        /// </summary>
        /// <param name="externMethod"></param>
        TResult ExecuteUsing<TResult>(Func<IRepository, TResult> externMethod);



        /// <summary>
        ///     Get the connection to the repository
        /// </summary>
        IDbConnection RepositoryConnection { get; }




        /// <summary>
        ///     Give the chance to execute some code before save is called.
        /// </summary>
        event Action<IRepository> ExaclyBeforeSaveCalled;
    }
}
