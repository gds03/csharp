using System;
using System.Collections.Generic;

namespace EnhancedLibrary.ExtensionMethods.Business
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
        public static TOut ReadKey<TIn, TOut>(this IDictionary<TIn, TOut> dictionary,
            TIn key, Func<TOut> callback) 
        {
            if ( !dictionary.ContainsKey(key) ) {
                dictionary.Add(key, callback());
            }

            return dictionary[key];
        }
    }
}
