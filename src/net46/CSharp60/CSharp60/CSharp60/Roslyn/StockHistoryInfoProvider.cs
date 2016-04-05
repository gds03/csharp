using CSharp60.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CSharp60.Roslyn
{
    public class StockHistoryInfoProvider
    {
        public Dictionary<string, List<StockInfo>> GetStockInformationByStockSymbol(string symbol)
        {
            var stocks = getAllStocksWithCSharp6Point0Version();



            if (stocks.Keys.Contains(symbol))
            {

                return
                    stocks.Where(stock => stock.Key == symbol).ToDictionary(key => key.Key, value => value.Value);
            }

            throw new ArgumentException("Stock Symbol is not found ");

        }

        private Dictionary<string, List<StockInfo>> getAllStocksWithCSharp6Point0Version()
        {
            return new Dictionary<string, List<StockInfo>>
            {
                ["AAPL"] = GetStockInfoUsingRosylnCompiler("AAPL"),
                ["BAC"] = GetStockInfoUsingRosylnCompiler("BAC"),
                ["AMBA"] = GetStockInfoUsingRosylnCompiler("AMBA"),
                ["GILD"] = GetStockInfoUsingRosylnCompiler("GILD"),
                ["FB"] = GetStockInfoUsingRosylnCompiler("FB"),
                ["TWTR"] = GetStockInfoUsingRosylnCompiler("TWTR"),
                ["DIS"] = GetStockInfoUsingRosylnCompiler("DIS")
            };
        }



        private List<StockInfo> GetStockInfoUsingRosylnCompiler(string stockSymbol)
        {
            string codeFile = File.ReadAllText("StockDataParser.txt");
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(codeFile);

            // Create Compilation
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var mscorelib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            // var stockinfolib = MetadataReference.CreateFromFile(typeof(StockInfo).Assembly.Location);

            var thisAssemblyReference = MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location);

            var linQAssemblyRefernce = MetadataReference.CreateFromFile(typeof(System.Linq.IOrderedQueryable).Assembly.Location);
            var compilation = CSharpCompilation.Create("StockDataParser")
                .WithOptions(options)
                .AddSyntaxTrees(syntaxTree)
                .AddReferences(mscorelib)
                .AddReferences(thisAssemblyReference)

                .AddReferences(linQAssemblyRefernce);

            // Show Diagnostics
            var diagnostics = compilation.GetDiagnostics();
            foreach (var item in diagnostics)
            {
                Console.WriteLine(item.ToString());
            }

            // Execute Code

            string fullClassName = "CSharp60.StockDataParser";
            using (var stream = new MemoryStream())
            {
                compilation.Emit(stream);
                var assembly = Assembly.Load(stream.GetBuffer());
                var type = assembly.GetType(fullClassName);

                // dynamic because type is in txt.
                dynamic stockInfo = Activator.CreateInstance(type);

                // call method name with dynamic
                return stockInfo.GetStockInfoByStockSymbol(stockSymbol);
            }
        }

    }
}