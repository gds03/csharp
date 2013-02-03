using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    public static class BoolExtensions
    {
        public static string ToBitString(this bool value)
        {
            return value ? "1" : "0";
        }
    }
}
