using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualNote.Common.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        public static String PrintSequence(this IEnumerable<String> collection)
        {
            var sb = new StringBuilder();
            foreach (String s in collection)
            {
                sb.Append(s + ", ");
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }
}
