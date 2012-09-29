using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnhancedLibrary.Utilities.Business;
namespace EnhancedLibrary.Tests
{
    
    [TestClass]
    public class TypeUtilsTest
    {
        class Aluno
        {
            public int ID { get; set; }
            public string Nome { get; set; }
            public string Morada { get; set; }
        }



       
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("Nome", Types.GetPropertyName<Aluno>(a => a.Nome));
        }
    }
}
