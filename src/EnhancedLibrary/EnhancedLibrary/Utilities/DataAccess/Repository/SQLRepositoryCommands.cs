using System;
using System.Data.Common;
using System.Data;
using EnhancedLibrary.Utilities.DataAccess.Repository.Interfaces;
using System.Collections.Generic;

namespace EnhancedLibrary.Utilities.DataAccess.Repository
{
    public class SQLRepositoryCommands : SQLRepository, ISQLRepositoryCommands
    {
        public SQLRepositoryCommands(DbConnection connection) : base(connection)
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

        public IEnumerable<T> ExecuteAndMap<T>(CommandType type, string commandText, params DbParameter[] parameters) where T : class
        {
            return base.Select<T>(type, commandText, parameters);
        }
    }
}
