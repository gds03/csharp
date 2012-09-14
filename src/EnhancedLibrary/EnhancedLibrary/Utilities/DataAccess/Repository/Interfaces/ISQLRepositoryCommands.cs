using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace EnhancedLibrary.Utilities.DataAccess.Repository.Interfaces
{
    public interface ISQLRepositoryCommands : IDisposable
    {
        IEnumerable<T> ExecuteAndMap<T>(CommandType type, String commandText, params DbParameter[] parameters) where T : class;

        int ExecuteCommand(CommandType type, String commandText, params DbParameter[] parameters);
        object ExecuteScalar(CommandType type, String commandText, params DbParameter[] parameters);

        DbConnection Connection { get; }
    }
}
