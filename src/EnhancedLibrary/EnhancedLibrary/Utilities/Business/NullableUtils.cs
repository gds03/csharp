using System;

namespace EnhancedLibrary.Utilities.Business
{
    public static class NullableUtils
    {
        /// <summary>
        ///     Transform to a nullable type the current type, if the value is the same as the value of consideratedNullValue.
        /// </summary>        
        public static Nullable<TValue1> GetNullableValue<TValue1, TValue2>(TValue1 value, TValue2 consideratedNullValue)
            where TValue1 : struct, IComparable
            where TValue2 : struct, IComparable 
        {
            if ( value.CompareTo(consideratedNullValue) == 0 )
                return new TValue1?();

            return new TValue1?(value);
        }
    }
}
