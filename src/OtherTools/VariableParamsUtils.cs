using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionMethods.Utilities
{
    public static class VariableParamsUtils
    {
        public static void ForEach<T>(Action<T> callback, params T[] items)
        {
            foreach ( T item in items )
                callback(item);
        }
    }
}
