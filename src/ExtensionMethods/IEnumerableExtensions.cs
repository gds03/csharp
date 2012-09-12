using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionMethods
{
    /// <summary>
    ///     Extends the behavior of the IEnumerable interface
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        ///     Iterate over the sequence and execute callback method for each element in the sequence
        /// </summary>
        /// <returns>The same sequence, allowing you to have a fluent API</returns>
        public static IEnumerable<T> Iterate<T>(this IEnumerable<T> enumeration, Action<T> callback)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");


            foreach (var i in enumeration)
                callback(i);

            return enumeration;
        }

        /// <summary>
        ///     Iterate over the sequence and execute callback method for each element in the sequence
        /// </summary>
        /// <returns>The same sequence, allowing you to have a fluent API</returns>
        public static IEnumerable<T> Iterate<T>(this IEnumerable<T> enumeration, Action<T, int> callback)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");

            int idx = 0;
            foreach (var item in enumeration)
            {
                callback(item, idx);
                idx++;
            }

            return enumeration;
        }


        /// <summary>
        ///     Iterate over the sequence and execute callback method for each element in the sequence
        /// </summary>
        /// <returns>The same sequence, allowing you to have a fluent API</returns>
        public static IEnumerable Iterate(this IEnumerable enumeration, Action<object> callback)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");


            foreach (var item in enumeration)
                callback(item);

            return enumeration;
        }


        /// <summary>
        ///     Iterate over the sequence and execute callback method for each element in the sequence
        /// </summary>
        /// <returns>The same sequence, allowing you to have a fluent API</returns>
        public static IEnumerable Iterate(this IEnumerable enumeration, Action<object, int> callback)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");

            int idx = 0;
            foreach (var item in enumeration)
            {
                callback(item, idx);
                idx++;
            }


            return enumeration;
        }






        /// <summary>
        ///     Builds a string with a representation of the sequence of the current items separated by delimiter character
        /// </summary>
        public static string DelimitWith<T>(this IEnumerable<T> items, char delimiter)
        {
            if ( items == null || items.Count() == 0 )
                return string.Empty;

            StringBuilder sbuilder = new StringBuilder();
            string delimiterStr = delimiter + " ";

            return items.Aggregate(sbuilder, (sb, i) => sb.Append(i.ToString() + delimiterStr)).Remove(sbuilder.Length - 2, 2).ToString();
        }
    }
}

