using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualNote.Common.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static Type GetFirstType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException();

            while (type.BaseType != null && type.BaseType != typeof(Object))
            {
                type = type.BaseType;
            }

            return type;
        }
    }
}
