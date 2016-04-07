using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Repository.ObjectMapper.Internal.CLR2SQL
{
    internal static class CLRTypes2SQLTypes
    {
        /// <summary>
        ///     Map CLR types into SQL Types
        /// </summary>
        internal static Dictionary<Type, string> GetConversions(int initialCapacity)
        {
            Debug.Assert(initialCapacity > 0);
            Dictionary<Type, string> conv = new Dictionary<Type, string>(initialCapacity);

            conv.Add(typeof(Boolean), "bit");
            conv.Add(typeof(Byte), "tinyint");
            conv.Add(typeof(Int16), "smallint");
            conv.Add(typeof(Int32), "int");
            conv.Add(typeof(Int64), "bigint");
            conv.Add(typeof(Decimal), "decimal");
            conv.Add(typeof(Single), "float");
            conv.Add(typeof(Double), "float");
            conv.Add(typeof(Enum), "int");
            conv.Add(typeof(Char), "smallint");
            conv.Add(typeof(String), "nvarchar(max)");
            conv.Add(typeof(Char[]), "nvarchar(max)");
            conv.Add(typeof(DateTime), "datetime");
            conv.Add(typeof(DateTimeOffset), "datetime2");
            conv.Add(typeof(TimeSpan), "time");
            conv.Add(typeof(Byte[]), "image");
            conv.Add(typeof(Guid), "uniqueidentifier");
            conv.Add(typeof(Object), "sql_variant");

            // For nullables
            conv.Add(typeof(Boolean?), "bit");
            conv.Add(typeof(Byte?), "tinyint");
            conv.Add(typeof(Int16?), "smallint");
            conv.Add(typeof(Int32?), "int");
            conv.Add(typeof(Int64?), "bigint");
            conv.Add(typeof(Decimal?), "decimal");
            conv.Add(typeof(Single?), "float");
            conv.Add(typeof(Double?), "float");
            conv.Add(typeof(Char?), "smallint");
            conv.Add(typeof(DateTime?), "datetime");
            conv.Add(typeof(DateTimeOffset?), "datetime2");
            conv.Add(typeof(TimeSpan?), "time");
            conv.Add(typeof(Guid?), "uniqueidentifier");

            return conv;
        }
    }
}
