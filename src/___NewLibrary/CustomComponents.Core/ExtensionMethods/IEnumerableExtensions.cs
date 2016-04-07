using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CustomComponents.Core.ExtensionMethods
{
    /// <summary>
    ///     Extends the behavior of the IEnumerable interface
    /// </summary>
    public static class IEnumerableExtensions
    {
        public static void ForEach<TElement>(this IEnumerable<TElement> source, Action<TElement> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (action == null)
                throw new ArgumentNullException("action");

            if (source.Count() == 0)
                return;

            foreach (TElement element in source)
            {
                action(element);
            }
        }






        /// <summary>
        ///     Builds a string with a representation of the sequence of the current items separated by delimiter character
        /// </summary>
        public static string DelimitWith<T>(this IEnumerable<T> items, char delimiter)
        {
            if (items == null || items.Count() == 0)
                return string.Empty;

            StringBuilder sbuilder = new StringBuilder();
            string delimiterStr = delimiter + " ";

            return items.Aggregate(sbuilder, (sb, i) => sb.Append(i.ToString() + delimiterStr)).Remove(sbuilder.Length - 2, 2).ToString();
        }






        public static IEnumerable<TSource> Except<TSource, TSource2>(this IEnumerable<TSource> source1,
            IEnumerable<TSource2> source2, Func<TSource, TSource2, bool> predicate)
        {
            return source1.Where(x => !source2.Any(y => predicate(x, y)));
        }





        public static IEnumerable<T1> Intersect<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2, Func<T1, T2, bool> predicate)
        {
            List<T1> result = new List<T1>();

            foreach (T1 item1 in e1)
            {
                bool found = false;

                foreach (T2 item2 in e2)
                {
                    if (predicate(item1, item2))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    result.Add(item1);
            }

            return result;
        }




        /// <summary>
        ///     Let you get some range of the items for a page.
        /// </summary>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int currentPage, int itemsPerPage)
        {
            if (source == null)
                return null;

            if (currentPage <= 0)
                throw new ArgumentException("currentPage <= 0 ");

            if (itemsPerPage <= 0)
                throw new ArgumentException("itemsPerPage <= 0");

            int bucket = (currentPage - 1) * itemsPerPage;

            if (bucket < 0)
                throw new InvalidOperationException();

            return source.Skip(bucket)
                        .Take(itemsPerPage)
                        .ToList();
        }



        /// <summary>
        ///     Let you get some range of the items for a specific property.
        /// </summary>
        public static IEnumerable<object> GetRangeInProjection<TSource>(this IEnumerable<TSource> enumerable, int startInclusive, int endExclusive, string propertyName)
        {
            if (enumerable == null || enumerable.Count() == 0)
                return null;

            if (startInclusive < 0)
                throw new ArgumentException("startInclusive < 0");

            if (endExclusive <= 0)
                throw new ArgumentException("endExclusive <= 0");

            if (endExclusive <= startInclusive)
                throw new InvalidOperationException("endExclusive <= startInclusive");

            if (startInclusive >= enumerable.Count())
                throw new InvalidOperationException("startInclusive >= enumerable.Count()");

            PropertyInfo pi = typeof(TSource).GetProperty(propertyName);

            if (pi == null)
                throw new InvalidOperationException("Property not found");

            return enumerable.Skip(startInclusive)
                             .Take((endExclusive - startInclusive))
                             .Select(s => s.GetType().GetProperty(pi.Name).GetValue(s, null))
                             .ToList();
        }

    }
}
