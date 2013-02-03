using System;
using System.Data.Common;
using System.Data;
using EnhancedLibrary.Utilities.DataAccess.Repository.Interfaces;
using System.Collections.Generic;

namespace EnhancedLibrary.Utilities.DataAccess.Repository
{
    public class SQLRepositoryCommands : SQLRepository, ISQLRepositoryCommands
    {
        public SQLRepositoryCommands(DbConnection connection, DbTransaction transaction = null) : base(connection, transaction)
        {

        }


        public int ExecuteCommand(CommandType type, String commandText, params DbParameter[] parameters)
        {
            return base.Execute(type, commandText, parameters);
        }

        public new object ExecuteScalar(CommandType type, String commandText, params DbParameter[] parameters)
        {
            return base.ExecuteScalar(type, commandText, parameters);
        }

        public IList<T> ExecuteAndMap<T>(CommandType type, string commandText, params DbParameter[] parameters) where T : class
        {
            return base.Select<T>(type, commandText, parameters);
        }

        public object[] ExecuteAndMap<T1, T2>(CommandType type, string commandText, params DbParameter[] parameters) 
            where T1 : class
            where T2 : class
        {
            return base.Select<T1, T2>(type, commandText, parameters);
        }
        public object[] ExecuteAndMap<T1, T2, T3>(CommandType type, string commandText, params DbParameter[] parameters)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return base.Select<T1, T2, T3>(type, commandText, parameters);
        }
    }
}
