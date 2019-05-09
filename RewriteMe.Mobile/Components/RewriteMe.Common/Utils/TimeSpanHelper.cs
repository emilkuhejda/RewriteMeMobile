using System;

namespace RewriteMe.Common.Utils
{
    public static class TimeSpanHelper
    {
        public static TimeSpan Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default(TimeSpan);

            if (TimeSpan.TryParse(value, out var time))
                return time;

            return default(TimeSpan);
        }
    }
}
