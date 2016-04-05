using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp60.Types
{
    public class StockInfo
    {
        public string Date { get; set; }
        public string OpenAt { get; set; }
        public string DayHigh { get; set; }
        public string DayLow { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }

        public override string ToString()
        {

            // var previousVersion = string.Format("{0} {1} {2} {3} {4} {5}", Date, OpenAt, DayHigh, DayLow, Close, Volume);
            return $"{Date} {OpenAt} {this.DayHigh}  {this.DayLow} {this.Close} {this.Volume}";
        }
    }
}
