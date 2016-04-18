using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.OMapper.Internal.Converters
{
    internal static class ValueToSQLConverter
    {
        /// <summary>
		///     Converts value into string correctly formatted and supported by SQL.
		/// </summary>
		internal static String Convert(object value)
        {
            if (value == null)
                return "NULL";

            // We must know the concrete type
            Type type = value.GetType();

            if (type == typeof(bool))
                return ((bool)value) ? "1" : "0";

            if (type == typeof(DateTime))
            {
                DateTime d = (DateTime)value;
                return "'" + d.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            if (type == typeof(Nullable<DateTime>))
            {
                DateTime? dn = (DateTime?)value;

                if (dn.HasValue)
                {
                    return "'" + dn.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                else
                {
                    return dn.ToString();
                }
            }

            if (type == typeof(Guid) || type == typeof(String) || type == typeof(Char) || type == typeof(Char[]))
                return "'" + value.ToString() + "'";


            // return normal
            return value.ToString();
        }
    }
}
