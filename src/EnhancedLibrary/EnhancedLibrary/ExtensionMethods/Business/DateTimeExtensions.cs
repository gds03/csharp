using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Gets a new Date at the last day of the current month of the date
        /// </summary>
        public static DateTime GetLastDay(this DateTime date)
        {
            DateTime dt = DateTime.Now;
            int dayAdd = 0;

            while ( ( dt = dt.AddDays(1) ).Month == date.Month )
                dayAdd++;

            return date.AddDays(dayAdd);
        }
    }
}
