using System;

namespace CustomComponents.Core.Types.ValueTypes
{
    public static class Enums
    {
        /// <summary>
        ///     Convert the TValue value to TEnum type
        /// </summary>
        /// <returns>TEnum type based on the TValue value</returns>
        public static TEnum ToEnum<TEnum, TSource>(TSource value)
            where TEnum : struct, IConvertible
            where TSource : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be a Enumerated Type");

            return (TEnum)Enum.ToObject(typeof(TEnum), value);

        }
    }
}