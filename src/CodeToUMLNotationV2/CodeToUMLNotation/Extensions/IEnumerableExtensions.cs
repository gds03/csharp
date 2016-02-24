using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        ///     Iterate over the collection and separate each element by text.
        /// </summary>
        public static StringBuilder SeparateBy<T>(this IEnumerable<T> ienumerable, string text)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty(text, "text");

            var sbuilder = ienumerable.Aggregate(new StringBuilder(), (sb, item) => sb.Append(item.ToString() + text));

            // remove last separator
            if (sbuilder.Length >= text.Length)
                sbuilder.Remove(sbuilder.Length - text.Length, text.Length);

            return sbuilder;
        }
    }
}
