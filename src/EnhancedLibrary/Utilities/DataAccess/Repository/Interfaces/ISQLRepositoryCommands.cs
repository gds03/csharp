using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace EnhancedLibrary.Utilities.DataAccess.Repository.Interfaces
{
    public interface ISQLRepositoryCommands : IDisposable
    {
        IList<T> ExecuteAndMap<T>(CommandType type, String commandText, params DbParameter[] parameters) where T : class;
        object[] ExecuteAndMap<T1, T2>(CommandType type, String commandText, params DbParameter[] parameters) where T1 : class where T2 : class;
        object[] ExecuteAndMap<T1, T2, T3>(CommandType type, String commandText, params DbParameter[] parameters)
            where T1 : class
            where T2 : class
            where T3 : class;

        int ExecuteCommand(CommandType type, String commandText, params DbParameter[] parameters);
        object ExecuteScalar(CommandType type, String commandText, params DbParameter[] parameters);

        DbConnection Connection { get; }
    }
}
