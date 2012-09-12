using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Reflection;
using ExtensionMethods;


namespace ModelShared
{
    public static class ObjectContextExtensions
    {
        /// <summary>
        ///     Return the SqlConnection that the context is associated with.
        /// </summary>
        public static SqlConnection GetSqlConnection(this ObjectContext context)
        {
            EntityConnection entityConn = context.Connection as EntityConnection;

            if (entityConn == null)
                throw new InvalidOperationException("Connection is not of EntityConnection type");

            SqlConnection sqlConn = entityConn.StoreConnection as SqlConnection;

            if (sqlConn == null)
                throw new InvalidOperationException("Connection in property StoreConnection is not of SqlConnection type");

            return sqlConn;
        }

		/// <summary>
		/// Starts a transaction within a context.
		/// </summary>
		public static SqlTransaction GetSqlTransaction(this ObjectContext context) {
			EntityTransaction eftran = context.Connection.StartEntityFrameworkTransaction();
			try { return eftran.GetSqlTransaction(); }
			catch {
				try { eftran.Rollback(); }
				catch { /* GLUP */ }
				throw;
			}
		}



    }
}
