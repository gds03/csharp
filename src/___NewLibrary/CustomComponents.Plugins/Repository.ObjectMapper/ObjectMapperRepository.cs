using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ObjectMapper
{
    public class ObjectMapperRepository : ObjectMapper, IRepository
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

        IRepository IRepository.Synchronize()
        {
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

        public void ExecuteBlock(Action<IRepository> externMethod, Action<Exception> exceptionMethod = null)
        {
            using (IRepository repository = this)
            {
                SqlTransaction transaction = null;

                try
                {
                    // Initialize transaction
                    transaction = ((SqlConnection)(repository.RepositoryConnection)).BeginTransaction();

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

        public TResult ExecuteUsing<TResult>(Func<IRepository, TResult> externMethod)
        {
            using (IRepository callerInstance = this)
            {
                return externMethod(callerInstance);
            }
        }

        public event Action<IRepository> ExaclyBeforeSaveCalled;
    }
}
