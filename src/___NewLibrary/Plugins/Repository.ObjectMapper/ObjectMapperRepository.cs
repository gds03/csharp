using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Interfaces;
using CustomComponents.Repository.Types.Generic;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ObjectMapper
{
    public class ObjectMapperRepository : ObjectMapper, IRepository, IDatabaseStored
    {
        public ObjectMapperRepository(string connectionString)
            : base(connectionString)
        {

        }

        public ObjectMapperRepository(DbConnection connection, DbTransaction transaction = null)
            : base(connection, transaction)
        {

        }


        public ObjectMapperRepository(DbConnection connection, int commandTimeout, DbTransaction transaction = null)
            : base(connection, commandTimeout, transaction)
        {

        }

        public event Callback ExaclyBeforeSaveCalled;


        public IQueryable<T> Query<T>() where T : class
        {
            return Select<T>().AsQueryable<T>();
        }

        public new IRepository Insert<T>(T e) where T : class
        {
            base.Insert(e);
            return this;
        }

        public new IRepository Delete<T>(T e) where T : class
        {
            base.Delete(e);
            return this;
        }


        public new IRepository Submit()
        {
            base.Submit();
            return this;
        }





        public IDbConnection RepositoryConnection
        {
            get { return base.Connection; }
        }


        QueryResult<T> IRepository.Query<T>()
        {
            throw new NotImplementedException();
        }

        public void ExecuteBlock(Callback externMethod, ExceptionCallback exceptionMethod = null)
        {
            using (IRepository repository = this)
            {
                IDatabaseStored db = this as IDatabaseStored;
                if (db == null)
                    throw new InvalidCastException("db");

                SqlTransaction transaction = null;

                try
                {
                    // Initialize transaction
                    transaction = ((SqlConnection)(db.RepositoryConnection)).BeginTransaction();

                    // Call user function
                    externMethod(repository);

                    // BLOCK DONE, PERSIST!
                    transaction.Commit();
                }

                catch (Exception e)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    if (exceptionMethod == null)
                        throw;

                    else { exceptionMethod(e); }
                }
            }
        }

        public TResult ExecuteUsing<TResult>(CallbackResult<TResult> externMethod)
        {
            using (IRepository callerInstance = this)
            {
                return externMethod(callerInstance);
            }
        }       
    }
}
