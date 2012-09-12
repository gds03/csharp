using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionMethods
{
    /// <summary>
    ///     Extends the behavior of the String interface
    /// </summary>
    public static class StringExtensions
    {
        readonly static Dictionary<Type, Func<String, object>> s_mappings = new Dictionary<Type, Func<String, object>>();

        static StringExtensions() 
        {
            // All are primitive types

            s_mappings.Add(typeof(Boolean), s => (s == "0") ? false : (s == "1") ? true : Boolean.Parse(s));
            s_mappings.Add(typeof(Byte), s => Byte.Parse(s));
            s_mappings.Add(typeof(Int16), s => Int16.Parse(s));
            s_mappings.Add(typeof(Int32), s => Int32.Parse(s));
            s_mappings.Add(typeof(Int64), s => Int64.Parse(s));
            s_mappings.Add(typeof(SByte), s => SByte.Parse(s));
            s_mappings.Add(typeof(UInt16), s => UInt16.Parse(s));
            s_mappings.Add(typeof(UInt32), s => UInt32.Parse(s));
            s_mappings.Add(typeof(UInt64), s => UInt64.Parse(s));
            s_mappings.Add(typeof(Decimal), s => Decimal.Parse(s));
            s_mappings.Add(typeof(Single), s => Single.Parse(s));
            s_mappings.Add(typeof(Double), s => Double.Parse(s));
            s_mappings.Add(typeof(DateTime), s => DateTime.Parse(s));
        }


        /// <summary>
        ///     Try parse the string value to specific struct.
        /// </summary>
        /// <returns>If the string is empty or null return nullable of TValue, otherwise return a nullable that is not null and contains the value</returns>
        public static Nullable<TValue> ToNullable<TValue>(this String value) where TValue : struct 
        {
            // If String is invalid, return nullable value with nothing in
            if ( string.IsNullOrEmpty(value) )
                return new TValue?();

            TValue v = (TValue) s_mappings[typeof(TValue)](value);
            return new TValue?(v);
        }


        /// <summary>
        ///     Try parse the string value to specific struct type
        /// </summary>
        /// <returns>The value of the string in the desired type</returns>
        public static TValue To<TValue>(this String value) where TValue : struct 
        {
            return (TValue) s_mappings[typeof(TValue)](value);
        }

        




        /// <summary>
        ///     Format the current string in the same way as the String.Format
        /// </summary>
        /// <returns>The currently string after formatted</returns>
        public static String Frmt(this String str, params object[] args) {
            return String.Format(str, args);
        }


        /// <summary>
        ///     Deprecated: You should use To<TValue> instead
        /// </summary>
        public static int ToInteger(this String str) {
            return (int) s_mappings[typeof(int)](str);
        }


        /// <summary>
        ///     Try convert a sequence of strings to integers
        /// </summary>
        /// <param name="str"></param>
        /// <returns>The sequence of the strings in a sequence of integers</returns>
        public static int[] ToIntegers(this String[] str) 
        {
            int[] r = new int[str.Length];

            for ( int x = 0; x < str.Length; x++ ) {
                r[x] = ToInteger(str[x]);
            }

            return r;
        }




        /// <summary>
        ///     Separate each value in the array with delimiter string and return a new string delimited.
        /// </summary>
        /// <returns>A new string delimited by the delimiter and the values</returns>
        public static string Delimit(this string[] values, string delimiter) {
            return values.Aggregate(new StringBuilder(), (sb, s) => sb.Append(s + delimiter)).ToString();
        }




        /// <summary>
        ///     Checks if the current string is null or is empty
        /// </summary>
        /// <returns>true if is null or empty, otherwise false</returns>
        public static bool IsNE(this string value)
        {
            return string.IsNullOrEmpty(value);
        }


        /// <summary>
        ///     Adds EndString to the end of the string if the string doesn't finish with that string.
        /// </summary>
        public static string AddAtEnd_If_NotAtTheEnd(this String value, string EndString)
        {
            return value.EndsWith(EndString) ? value : ( value + EndString);
        }

        /// <summary>
        ///     Remove EndString to the end of the string if the string finish with that string.
        /// </summary>
        public static string RemoveAtEnd_If_AtTheEnd(this String value, string EndString)
        {
            int idx = value.LastIndexOf(EndString);
            if( idx == -1 )
                return value;

            return value.Substring(0, idx);
        }
    }
}
