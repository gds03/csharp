using CustomComponents.Database.Types.Generic;
using CustomComponents.Repository.Interfaces;
using CustomComponents.Repository.Types.Generic;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repository.OMapper
{
    public class OMapperRepository : IRepository, IDatabaseStored
    {
        private readonly OMapperContextExecuter m_oMapper;


        public OMapperRepository(string connectionString)
        {
            m_oMapper = new OMapperContextExecuter(connectionString, IsolationLevel.ReadCommitted);
        }


        public OMapperRepository(OMapperContextExecuter oMapperExecuter)
        {
            if (oMapperExecuter == null)
                throw new ArgumentNullException("oMapperExecuter");

            m_oMapper = oMapperExecuter;
        }


        public event Callback ExaclyBeforeSaveCalled;


        

        public IRepository Insert<T>(T e) where T : class
        {
            m_oMapper.Insert(e);
            return this;
        }

        public IRepository Delete<T>(T e) where T : class
        {
            m_oMapper.Delete(e);
            return this;
        }


        public IRepository Submit()
        {
            m_oMapper.Submit();
            return this;
        }





        public IDbConnection RepositoryConnection
        {
            get { return m_oMapper.Connection; }
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



        public IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return m_oMapper.Select(predicate);
        }



        QueryResult<T> IRepository.Query<T>()
        {
            return new QueryResult<T>(this.Query<T>());
        }

        public void Dispose()
        {
            m_oMapper.Dispose();
        }










        private IQueryable<T> Query<T>() where T : class
        {
            return m_oMapper.Select<T>().AsQueryable<T>();
        }

       
    }
}
