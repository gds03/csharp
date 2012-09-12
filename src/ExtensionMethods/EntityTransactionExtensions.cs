using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Reflection;

namespace ExtensionMethods
{
    public static class EntityTransactionExtensions
    {
        /// <summary>
        ///     Get SqlTransaction that entityTransaction object is associated with.
        /// </summary>
        public static SqlTransaction GetSqlTransaction(this EntityTransaction entityTransaction)
        {
            // Get flags for get the member
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic;

            // Get transaction from EntityTransaction
            SqlTransaction sqlTran = (SqlTransaction)entityTransaction.GetType()
                                                                      .InvokeMember("StoreTransaction",
                                                                                     flags,
                                                                                     null,
                                                                                     entityTransaction,
                                                                                     new object[0]
                                    );

            return sqlTran;
        }
    }
}
