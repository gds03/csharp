using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnhancedLibrary.Utilities.Business;

namespace EnhancedLibrary.Tests
{
    internal class MyType1
    {
        public int Property1 { get; set; }
        public String Property2 { get; set; }
        public bool Property3 { get; set; }
        public char Property4 { get; set; }
    }

    internal class MyType2
    {
        public int Property1 { get; set; }
        public String Property2 { get; set; }
        public bool Property3 { get; set; }
    }


    [TestClass]
    public class TwoTypesUtilitiesClassTest
    {




        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void DiferentTypes()
        {


            MyType1 t1 = new MyType1();
            MyType2 t2 = new MyType2();

            IEnumerable<String> result = TwoTypesUtilitiesClass.GetChangedProperties(t1, t2);

            //
            // We should never be here
            // 

            Assert.Fail();
        }


        [TestMethod]
        public void SameTypesDiferentData()
        {


            MyType1 t1 = new MyType1()
            {
                Property1 = 20,
                Property2 = "Goncalo",
                Property3 = true,
                Property4 = 'g'
            };

            MyType1 t2 = new MyType1()
            {
                Property1 = 10,
                Property2 = "Dias",
                Property3 = true,
                Property4 = '3'
            };

            var result = TwoTypesUtilitiesClass.GetChangedProperties(t1, t2);

            Assert.IsTrue(result.Count() == 3);

            Assert.IsTrue(result.All(s => s == "Property1" || s == "Property2" || s == "Property4"));
        }



        [TestMethod]
        public void SameTypesSameData()
        {


            MyType1 t1 = new MyType1()
            {
                Property1 = 20,
                Property2 = "Goncalo",
                Property3 = true,
                Property4 = 'g'
            };

            MyType1 t2 = new MyType1()
            {
                Property1 = 20,
                Property2 = "Goncalo",
                Property3 = true,
                Property4 = 'g'
            };

            var result = TwoTypesUtilitiesClass.GetChangedProperties(t1, t2);

            Assert.IsTrue(result.Count() == 0);
        }
    }

}
