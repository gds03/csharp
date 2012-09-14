using System;
using System.Data.EntityClient;
using System.Data;

namespace EnhancedLibrary.ExtensionMethods.DataAccess
{
    public static class IDbConnectionExtensions
    {
        /// <summary>
        ///     Try to start a transaction, with a specified isolation level, in a context of an entity framework connection.
        ///     You must explicitly commit the transaction when you're done
        /// </summary>
        public static EntityTransaction StartEntityFrameworkTransaction(this IDbConnection connection, IsolationLevel level)
        {
            // Prepare entity framework connection
            EntityConnection efConnection = connection as EntityConnection;

            if ( efConnection == null )
                throw new InvalidOperationException(string.Format("connection must be of {0} type", typeof(EntityConnection).Name));

            efConnection.Open();
            return efConnection.BeginTransaction(level);
        }



        /// <summary>
        ///     Try to start a transaction in a context of an entity framework connection.
        ///     You must explicitly commit the transaction when you're done
        /// </summary>
        public static EntityTransaction StartEntityFrameworkTransaction(this IDbConnection connection)
        {
            return StartEntityFrameworkTransaction(connection, IsolationLevel.ReadCommitted);
        }
    }
}
