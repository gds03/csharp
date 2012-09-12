using System;
using System.Text;

namespace ExtensionMethods.Utilities
{
    public static class StringUtils
    {
        /// <summary>
        ///     Separate each value in values array by commas
        /// </summary>
        /// <returns>A formatted string with values and commas</returns>
        public static String SeparateByCommas<T>(T[] values) 
        {
            if ( values == null || values.Length == 0 )
                return "";

            StringBuilder sb = new StringBuilder();

            
            int i = 0;

            for ( ; i < values.Length; i++ ) {
                sb.Append(values[i].ToString());
                sb.Append(", ");
            }

            if ( i > 0 )
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }
    }
}
