using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnhancedLibrary.Utilities.Business
{
    public static class TwoTypesUtilitiesClass
    {
        /// <summary>
        ///     Compare to instances of the same type and return the name of the properties that were changed
        /// </summary>
        /// <returns>The name of the properties where they value is different</returns>
        public static IEnumerable<String> GetChangedProperties<TEntity1, TEntity2>(TEntity1 original, TEntity2 changed) 
        {
            if ( typeof(TEntity1) != typeof(TEntity2) )
                throw new InvalidOperationException("Types must be identical");

            //
            // If entities are of the same type, they have the same number of properties
            //

            Type repOriginal = original.GetType();
            Type repChanged = changed.GetType();

            if(repOriginal.IsValueType)
                throw new InvalidOperationException("Value types are not allowed to be compared");

            var propertiesInfo = repOriginal.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // Iterate over propertiesInfo and change the projection
            return propertiesInfo.Where(pi => pi.GetValue(original, null).Equals(repChanged.GetProperty(pi.Name).GetValue(changed, null)) == false)
                                 .Select(pi => pi.Name)
                                 .ToList();

        }
    }
}
