using CustomComponents.Core.Types.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.UnitTesting
{
    public static class AssertHelpers
    {
        private static object GetValues<T>(T a, T b, Expression<Func<T, object>> propertySelector, out object ValueB)
        {
            // [FR]: Linhas comentadas para efeitos de compilação.
            object ValueA = NamesResolver.Property<T>(a, propertySelector).Value; //.Param1;
            ValueB = NamesResolver.Property<T>(b, propertySelector).Value; //.Param1;

            if (ValueA != null && ValueA.GetType() == typeof(DateTime))
            {
                ValueA = ValueA.ToString();
                ValueB = ValueB.ToString();
            }

            return ValueA;
        }



        //
        // Public Assert Helpers

        public static void AreEqual<T>(T a, T b, Expression<Func<T, object>> propertySelector)
        {
            object valueB;
            object valueA = GetValues(a, b, propertySelector, out valueB);

            Assert.AreEqual(valueA, valueB);
        }

        public static void AreNotEqual<T>(T a, T b, Expression<Func<T, object>> propertySelector)
        {
            object valueB;
            object valueA = GetValues(a, b, propertySelector, out valueB);

            Assert.AreNotEqual(valueA, valueB);
        }



        public static void AreEqual(DateTime a, DateTime b)
        {
            Assert.AreEqual<string>(a.ToString(), b.ToString());
        }
    }
}
