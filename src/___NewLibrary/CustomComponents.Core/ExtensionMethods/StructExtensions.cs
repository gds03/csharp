using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class StructExtensions
    {
        /// <summary>
        ///     Convert the TValue value to TEnum type
        /// </summary>
        /// <returns>TEnum type based on the TValue value</returns>
        public static TEnum ToEnum<TEnum, TSource>(this TSource value)
            where TEnum : struct, IConvertible
            where TSource : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be a Enumerated Type");

            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }
    }
}
