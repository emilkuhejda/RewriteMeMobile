using System;

namespace RewriteMe.Common.Utils
{
    public static class EnumHelper
    {
        public static T Parse<T>(string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }
    }
}
