using System;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Gets a new Date at the last day of the current month, and if removeTimePart, the time part will be removed
        /// </summary>
        public static DateTime GetLastDayOfCurrentMonth(this DateTime date, bool removeTimePart)
        {
            return _GetLastDayOfCurrentMonth(date, removeTimePart);
        }


        /// <summary>
        ///     
        /// </summary>
        public static DateTime GetLastDayOfCurrentMonth(this DateTime date)
        {
            return _GetLastDayOfCurrentMonth(date, false);
        }



        public static DateTime RemoveTimePart(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(-1);
        }






        /// <summary>
        ///     Gets a new Date at the last day of the current month of the date
        /// </summary>
        private static DateTime _GetLastDayOfCurrentMonth(DateTime date, bool removeTimePart)
        {
            DateTime d = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            TimeSpan t;

            if (!removeTimePart)
            {
                // <date> hh:mm:ss
                t = new TimeSpan(date.Hour, date.Minute, date.Second);
            }

            else
            {
                // <date> 23:59:59
                t = new TimeSpan(23, 59, 59);
            }

            return d.Add(t);
        }


        public static string GetTimePart(this DateTime dt)
        {
            return dt.ToString("hh:mm");
        }

        //
        //DateTime To UnixTimeStamp
        public static double ToUnixTimeStamp(this DateTime dt)
        {
            return (dt - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

    }
}