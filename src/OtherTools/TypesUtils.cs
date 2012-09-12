using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ExtensionMethods.Utilities
{

    /// <summary>
    ///     
    /// </summary>
    public static class TypesUtils
    {
        /// <summary>
        ///     Set value in each item on him property with the propertyName .
        /// </summary>
        public static void ForEachSetPropertyValue<T>(Expression<Func<T, object>> propertyName, object value, params T[] items)
        {
            string pName = ((MemberExpression)((UnaryExpression)propertyName.Body).Operand).Member.Name;
           
            foreach ( var c in items ) 
            {
                c.GetType().GetProperty(pName, BindingFlags.Public | BindingFlags.Instance)
                           .SetValue(c, value, null);        
            }
        }
    }
}
