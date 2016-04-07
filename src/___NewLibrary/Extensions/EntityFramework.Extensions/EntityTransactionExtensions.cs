using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Extensions
{
    /// <summary>
    ///     Extends the behavior of the EntityTransaction class.
    /// </summary>
    public static class EntityTransactionExtensions
    {
        /// <summary>
        ///     Get SqlTransaction that entityTransaction object is associated with.
        /// </summary>
        public static SqlTransaction GetSqlTransaction(this EntityTransaction entityTransaction)
        {
            // Get flags for get the member
            BindingFlags flags = BindingFlags.FlattenHierarchy |
                                 BindingFlags.NonPublic |
                                 BindingFlags.InvokeMethod |
                                 BindingFlags.Instance |
                                 BindingFlags.GetProperty;

            // Get transaction from EntityTransaction
            var sqlTran = (SqlTransaction)entityTransaction.GetType()
                                           .InvokeMember("StoreTransaction",
                                           flags,
                                           null,                                // default binder
                                           entityTransaction,                   // context object
                                           new object[0]                        // parameters (none)
                                    );

            return sqlTran;
        }
    }
}
