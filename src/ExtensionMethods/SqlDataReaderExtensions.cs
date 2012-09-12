using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ExtensionMethods
{
    public static class SqlDataReaderExtensions
    {
        public static T? GetNullable<T>(this SqlDataReader reader, int columnIndex) where T : struct
        {
            T value = (T) reader[columnIndex];
            return new Nullable<T>(value);
        }

        public static T? GetNullable<T>(this SqlDataReader reader, string columnName) where T : struct
        {
            T value = (T) reader[columnName];
            return new Nullable<T>(value);
        }

        public static string GetNullableString(this SqlDataReader reader, int columnIndex)
        {
            if ( reader.IsDBNull(columnIndex) )
                return string.Empty;

            return reader.GetString(columnIndex);
        }

        public static string GetNullableString(this SqlDataReader reader, string columnName) 
        {
            int ordinal = reader.GetOrdinal(columnName);
            return GetNullableString(reader, ordinal);
        }
    }
}
