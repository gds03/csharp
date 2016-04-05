using CSharp60.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;
using CSharp60.Roslyn;

namespace CSharp60
{
    class Program
    {
        static void Main(string[] args)
        {
            var product = new Product();

            WriteLine(product.ProductId);
            WriteLine(product.DisplayProduct());
            WriteLine(product.ProductInfo);

            RoslynTest.loadStockDataByStockSymbol("DIS");

            ReadLine();
        }



    }
}

