using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace ExtensionMethods
{
    public static class IDbConnectionExtensions
    {
        /// <summary>
        ///     Try to start a transaction in a context of an entity framework connection.
        ///     You must explicitly commit the transaction when you're done
        /// </summary>
        public static EntityTransaction StartEntityFrameworkTransaction(this IDbConnection connection)
        {
            // Prepare entity framework connection
            EntityConnection efConnection = connection as EntityConnection;

            if (efConnection == null)
                throw new InvalidOperationException(string.Format("connection must be of {0} type", typeof(EntityConnection).Name));
			
            efConnection.Open();
            return efConnection.BeginTransaction();
        }

        /// <summary>
        ///     Try to start a transaction, with a specified isolation level, in a context of an entity framework connection.
        ///     You must explicitly commit the transaction when you're done
        /// </summary>
        public static EntityTransaction StartEntityFrameworkTransaction(this IDbConnection connection, IsolationLevel iso) {
            // Prepare entity framework connection
            EntityConnection efConnection = connection as EntityConnection;

            if (efConnection == null)
                throw new InvalidOperationException(string.Format("connection must be of {0} type", typeof(EntityConnection).Name));

            efConnection.Open();
            return efConnection.BeginTransaction(iso);
        }

    }
}
