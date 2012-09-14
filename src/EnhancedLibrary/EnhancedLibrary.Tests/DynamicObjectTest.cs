using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnhancedLibrary.Types.DynamicData;

namespace EnhancedLibrary.Tests
{
    [TestClass]
    public class DynamicObjectTest
    {
        [TestMethod]
        public void TestAddSomeProperties()
        {
            DynamicObject obj1 = new DynamicObject();

            obj1["Nome"] = "Locarent";
            obj1["Concelho"] = "Oeiras";
            obj1["Numero"] = 12345;

            obj1.SetPropertyValueAndEvent("Morada", "Lagoas Park frente ao Lago", (k, x) =>
            {
                Assert.AreEqual(k, "Morada");
                Assert.AreEqual(x, "Lagoas Park frente ao Lago");
            });

            Assert.AreEqual(obj1["Nome"], "Locarent");
            Assert.AreEqual(obj1["Numero"], 12345);

            obj1["Morada"] = "Vai chamar o callback";        
        }
    }
}
