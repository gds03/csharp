using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ExtensionMethods
{
    public static class ICloneableExtensions
    {
        /// <summary>
        ///     
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

        static bool IsValidType(PropertyInfo pi)
        {
            var t = pi.PropertyType;

            return pi.CanWrite && ( ( t.IsPrimitive ) || ( ( t.IsPrimitive == false ) && t == typeof(string) ) || ( t.IsValueType ) );
        }
    }
}
