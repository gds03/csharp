
using System;
namespace EnhancedLibrary.Utilities.Business
{
    public static class Time
    {
        public static int SecondsToMilis(int seconds) { return seconds * 1000; }
        public static int MinutesToMilis(int minutes) { return minutes * SecondsToMilis(60); }
        public static int HoursToMilis(int hours) { return hours * MinutesToMilis(60); }
        public static int DaysToMilis(int days) { return days * HoursToMilis(24); }


        public static int MilisToSeconds(int milis) { return milis / 1000; }
        public static int MilisToMinutes(int milis) { return milis / 60000; }
        public static int MilisToHours(int milis) { return milis / 3600000; }
        public static int MilisToDays(int milis) { return milis / 86400000; }

    }
}
