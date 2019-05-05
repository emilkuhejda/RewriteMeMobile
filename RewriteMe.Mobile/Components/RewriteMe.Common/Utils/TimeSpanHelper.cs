using System;

namespace RewriteMe.Common.Utils
{
    public class TimeSpanHelper
    {
        public static TimeSpan Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default(TimeSpan);

            TimeSpan.TryParse(value, out var time);
            return time;
        }
    }
}
