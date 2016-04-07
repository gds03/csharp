using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlExtensions.Extensions
{
    /// <summary>
    ///     Extends the behavior of the SqlDataReader class.
    /// </summary>
    public static class SqlDataReaderExtensions
    {

        public static T? GetNullable<T>(this SqlDataReader reader, int columnIndex) where T : struct
        {
            T value = (T)reader[columnIndex];
            return new Nullable<T>(value);
        }

        public static T? GetNullable<T>(this SqlDataReader reader, string columnName) where T : struct
        {
            T value = (T)reader[columnName];
            return new Nullable<T>(value);
        }

        /// <summary>
        ///     Use this method, when you're getting a columnName that are of string type and are null.
        ///     With this method, you don't receive an exception
        /// </summary>
        public static string GetNullableString(this SqlDataReader reader, int columnIndex)
        {
            if (reader.IsDBNull(columnIndex))
                return string.Empty;

            return reader.GetString(columnIndex);
        }

        /// <summary>
        ///     Use this method, when you're getting a columnName that are of string type and are null.
        ///     With this method, you don't receive an exception
        /// </summary>
        public static string GetNullableString(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return GetNullableString(reader, ordinal);
        }
    }
}
