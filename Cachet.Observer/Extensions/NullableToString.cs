using System;
using System.Collections.Generic;
using System.Text;

namespace CachetObserver.Extensions
{
    public static class NullHelpers
    {
        public static string NullableToString(this string value)
        {
            if (value == null)
            {
                return "null";
            }
            return value;
        }
        public static string NullableToString(this Uri value)
        {
            if (value == null)
            {
                return "null";
            }
            return value.ToString();
        }

        public static string TrySubstring(this string value, int startIndex, int length, bool returnvalue = false)
        {
            try
            {
                return value.Substring(startIndex, length);
            }
            catch
            {
                if (returnvalue)
                {
                    return value;
                }
                return null;
            }
        }
    }
}
