using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types
{
    /// <summary>
    ///     Overrides the behavior of ToString() of DateTime struct, 
    ///     formatting to the correct application date format.
    /// </summary>
    public struct CustomDateTime : IComparable, IComparable<CustomDateTime>
    {
        public DateTime Date;
        public bool IgnoreTime;

        public CustomDateTime(DateTime Date, bool IgnoreTime = false)
        {
            DateTime dt = (IgnoreTime) ? Date.Date : Date;      // if is to ignore the time, consider only the date part.
            this.Date = dt;
            this.IgnoreTime = IgnoreTime;
        }


        public override string ToString()
        {
            // override default format
            return (IgnoreTime) ? Date.ToString("dd-MM-yyyy") : (Date.ToString("dd-MM-yyyy HH:mm"));
        }


        public int CompareTo(CustomDateTime other)
        {
            // 
            // use the DateTime compareTo (CORE comparer)

            return this.Date.CompareTo(other.Date);
        }


        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(CustomDateTime))
            {
                CustomDateTime a = (CustomDateTime)obj;
                return this.CompareTo(a);
            }

            if (obj.GetType() == typeof(DateTime))
            {
                DateTime b = (DateTime)obj;
                return this.CompareTo(new CustomDateTime(b, this.IgnoreTime));      // if current is to ignore, the other is too.
            }

            // not supported
            return -1;
        }
    }
}
