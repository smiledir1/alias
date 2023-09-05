using System;

namespace Common.Utils
{
    public static class TimeUtils
    {
        public static int GetYears(this TimeSpan timespan) => 
            (int) (timespan.Days / 365.2425);

        public static int GetMonths(this TimeSpan timespan) => 
            (int) (timespan.Days / 30.436875);

        public static TimeSpan TimeToNextDay()
        {
            var now = DateTime.Now;
            var dayEnd = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999);
            return dayEnd - now;
        }

        public static string SecondsToDuration(TimeSpan timespan)
        {
            if (timespan.Hours > 0)
            {
                return $"{timespan.Hours}:{timespan.Minutes}:{timespan.Seconds}";
            }
            
            if (timespan.Minutes > 0)
            {
                return $"{timespan.Minutes}:{timespan.Seconds}";
            }
            
            if (timespan.Seconds > 0)
            {
                return $"{timespan.Seconds}";    
            }

            return string.Empty;
        }
    }
}