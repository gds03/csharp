using System;
using System.Collections.Generic;

namespace CustomComponents.Core.ExtensionMethods
{
    /// <summary>
    ///     Extends the behavior of the IDictionary Interface
    /// </summary>
    public static class IDictionaryExtensions
    {


        /// <summary>
        ///     Read the key of the dictionary, and if the key is not present on the dictionary, call
        ///     callback function and set the key from the result of that function.
        /// </summary>
        /// <returns>The value on the specific key</returns>
        public static TValue ReadKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key, Func<TValue> callback)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, callback());
            }

            return dictionary[key];
        }
    }
}