using System;

namespace ExtensionMethods
{
    public static class EnumUtils
    {
        /// <summary>
        ///     Convert the TValue value to TEnum type
        /// </summary>
        /// <returns>TEnum type based on the TValue value</returns>
        public static TEnum ToEnum<TEnum, TValue>(TValue value) 
            where TEnum : struct, IConvertible
            where TValue : struct
        {
            if ( !typeof(TEnum).IsEnum )
                throw new ArgumentException("TEnum must be a Enumerated Type");

            return (TEnum) Enum.ToObject(typeof(TEnum), value);

        }
    }
}
