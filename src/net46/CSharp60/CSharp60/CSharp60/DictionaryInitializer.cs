using CSharp60.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp60
{
    public static class DictionaryInitializer
    {
        public static Dictionary<string, List<StockInfo>> GetStockNewVersion()
        {
            return new Dictionary<string, List<StockInfo>>
            {
                ["stock1"] = new List<StockInfo>
                {
                    new StockInfo { Date = DateTime.Now.ToShortDateString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                    new StockInfo { Date = DateTime.Now.ToShortDateString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                },

                ["stock2"] = new List<StockInfo>
                {
                    new StockInfo { Date = DateTime.Now.ToShortDateString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                    new StockInfo { Date = DateTime.Now.ToShortDateString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                }
            };
        }


        public static Dictionary<string, List<StockInfo>> GetStockOldVersion()
        {
            return new Dictionary<string, List<StockInfo>>
            {
                { "stock1", new List<StockInfo> {
                    new StockInfo { Date = DateTime.Now.ToShortDateString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                    new StockInfo { Date = DateTime.Now.ToString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                    }
                },

                { "stock2", new List<StockInfo> {
                    new StockInfo { Date = DateTime.Now.ToShortDateString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                    new StockInfo { Date = DateTime.Now.ToString(), Close = 101.20.ToString(), DayHigh = 101.ToString(), DayLow = 60.ToString() },
                    }
                }
            };
        }
    }
}
