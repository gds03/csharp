using System;
using System.Collections.Generic;

namespace VirtualNote.Common.ExtensionMethods
{

    public static class DictionaryExtensions
    {
        public static TValue ReturnOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key, Func<TValue> function)
        {
            if (dictionary == null)
                throw new ArgumentNullException();

            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = function();
            }

            return dictionary[key];
        }
    }
}
