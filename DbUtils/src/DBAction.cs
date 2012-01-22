using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DbUtils
{
    public sealed class DbAction
    {
        private readonly DbConnection _connection;


        public DbAction(DbConnection connection)
        {
            _connection = connection;
        }



        public DbDataReader Query(CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if(_connection.State == ConnectionState.Closed)
                _connection.Open();

            DbCommand comm = _connection.CreateCommand();
            
            comm.CommandType = commandType;
            comm.CommandText = commandText;

            // Set parameters
            if ( parameters != null ) {
                comm.Parameters.AddRange(parameters);
            }

            return comm.ExecuteReader();
        }






    }
}
