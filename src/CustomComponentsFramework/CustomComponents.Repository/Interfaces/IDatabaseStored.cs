using CustomComponents.Repository.Types.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CustomComponents.Repository.Interfaces
{
    public interface IDatabaseStored
    {
        /// <summary>
        ///     Submit Changes into database.
        /// </summary>
        IRepository Submit();


        /// <summary>
        ///     Execute user extern method within a transaction of a repository
        /// </summary>
        /// <param name="externMethod"></param>
        void ExecuteBlock(Callback externMethod, ExceptionCallback exceptionMethod = null);


        /// <summary>
        ///     Free the developer to use Using statement
        /// </summary>
        /// <param name="externMethod"></param>
        TResult ExecuteUsing<TResult>(CallbackResult<TResult> externMethod);



        /// <summary>
        ///     Get the connection to the repository
        /// </summary>
        IDbConnection RepositoryConnection { get; }




        /// <summary>
        ///     Give the chance to execute some code before save is called.
        /// </summary>
        event Callback ExaclyBeforeSaveCalled;
    }
}
