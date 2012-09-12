using System.Data;
using System.Data.SqlClient;

namespace ExtensionMethods
{
    /// <summary>
    ///     Extends the behavior of the SqlParameter interface
    /// </summary>
    public static class SqlParameterExtensions
    {

        /// <summary>
        ///     Make the specific SqlParameter an output parameter
        /// </summary>
        public static SqlParameter MakeOutputParameter(this SqlParameter parameter) {
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }




        /// <summary>
        ///     Set the specific SqlParameter direction
        /// </summary>
        public static SqlParameter SetOutputDirection(this SqlParameter parameter, ParameterDirection direction)
        {
            parameter.Direction = direction;
            return parameter;
        } 
    }
}
