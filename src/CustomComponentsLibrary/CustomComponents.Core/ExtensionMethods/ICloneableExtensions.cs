using System;
using System.Reflection;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class ICloneableExtensions
    {
        /// <summary>
        ///     Clone the current obj object properties that are Valuetypes and Strings.
        /// </summary>
        static void _SuperficialClone<T>(this T current, T newObj)
            where T : class, ICloneable
        {
            var objProperties = current.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in objProperties)
            {
                if (IsValidType(property))
                {
                    // Get value from obj property
                    object val = property.GetValue(current, null);

                    // Set value to result property
                    newObj.GetType().GetProperty(property.Name).SetValue(newObj, val, null);
                }
            }
        }

        /// <summary>
        ///     Copies the current properties values to the newObject
        /// </summary>
        public static void SuperficialClone<T>(this T current, T newObj)
            where T : class, ICloneable
        {
            _SuperficialClone(current, newObj);
        }

        /// <summary>
        ///     Creates a new object with the copy of the properties of current object.
        /// </summary>
        public static T SuperficialClone<T>(this T current)
            where T : class, ICloneable
        {
            T newObj = Activator.CreateInstance<T>();
            _SuperficialClone(current, newObj);
            return newObj;
        }







        #region Internal methods


        static bool IsValidType(PropertyInfo pi)
        {
            var t = pi.PropertyType;

            return pi.CanWrite && ((t.IsPrimitive) || ((t.IsPrimitive == false) && t == typeof(string)) || (t.IsValueType));
        }


        #endregion
    }
}