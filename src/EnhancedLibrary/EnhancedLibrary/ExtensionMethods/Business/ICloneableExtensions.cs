using System;
using System.Reflection;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    public static class ICloneableExtensions
    {
        /// <summary>
        ///     Clone the current obj object properties that are Valuetypes and Strings.
        /// </summary>
        public static T SuperficialClone<T>(this T obj) 
            where T : class, ICloneable
        {
            T result = Activator.CreateInstance<T>();

            var objProperties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);


            foreach ( var property in objProperties )
            {
                if ( IsValidType(property) )
                {
                    // Get value from obj property
                    object val = property.GetValue(obj, null);

                    // Set value to result property
                    result.GetType().GetProperty(property.Name).SetValue(result, val, null);
                }
            }

            return result;
        }





        #region Internal methods
        

        static bool IsValidType(PropertyInfo pi)
        {
            var t = pi.PropertyType;

            return pi.CanWrite && ( ( t.IsPrimitive ) || ( ( t.IsPrimitive == false ) && t == typeof(string) ) || ( t.IsValueType ) );
        }


        #endregion
    }
}
