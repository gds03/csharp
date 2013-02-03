using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using EnhancedLibrary.Utilities.DataAccess.Repository.Interfaces;

namespace EnhancedLibrary.Utilities.DataAccess.Repository
{
    public class SQLRepository : ObjectMapper, ISQLRepository
    {
        public SQLRepository(DbConnection connection, DbTransaction transaction = null) : base(connection, transaction)
        {

        }


        #region Query
        


        public T QuerySingle<T>(Expression<Func<T, bool>> filter) where T : class
        {
            return base.Select(filter).Single();
        }

        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> filter) where T : class
        {
            return base.Select(filter);
        }
       

        public IEnumerable<T> Query<T>() where T : class
        {
            return base.Select<T>();
        }



        #endregion







        public new void Insert<T>(T e) where T : class
        {
            base.Insert(e);
        }

        public new void Update<T>(T e) where T : class
        {
            base.Update(e);
        }

        public new void Delete<T>(T e) where T : class
        {
            base.Delete(e);
        }

        public new DbConnection Connection
        {
            get
            {
                return base.Connection;
            }
        }
    }
}
