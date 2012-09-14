using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedLibrary.Utilities.Business
{
    public static class UnitUtils
    {
        public static string AddEuro(object value) { return value.ToString() + " €"; }
        public static string AddKms(object value) { return value.ToString() + " Kms"; }
        public static string AddLiters(object value) { return value.ToString() + " Lt."; }
        public static string AddEuroPerLiters(object value) { return value.ToString() + " €/Lt."; }
    }
}
