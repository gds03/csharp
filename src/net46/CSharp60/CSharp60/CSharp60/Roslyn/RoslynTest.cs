using CSharp60.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp60.Roslyn
{
    public static class RoslynTest
    {

        public static void loadStockDataByStockSymbol(string stockTickerSymbol)
        {
            var stockHistoryProvider = new StockHistoryInfoProvider();

            var stockSymbols = stockHistoryProvider.GetStockInformationByStockSymbol(stockTickerSymbol);

            printTickerSymbol(stockSymbols);
        }



        private static void printTickerSymbol(Dictionary<string, List<StockInfo>> stockInfos)
        {
            if (stockInfos.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                var stockInformation = stockInfos.Select(dic => dic.Value).SingleOrDefault();

                Console.WriteLine($"{"Date"} {"Day High"} {"Day Low"} {"Open At"} {"Close"} {"Volume"} {"\n"}");                
                Console.WriteLine("\n");

                foreach (StockInfo sf in stockInformation)
                {
                    var stockInfo = $"{sf.Date} {sf.DayHigh} { sf.DayLow} {sf.OpenAt} {sf.Close} {sf.Volume} {"\n"}";

                    Console.WriteLine(stockInfo);
                }
            }
        }

    }
}
