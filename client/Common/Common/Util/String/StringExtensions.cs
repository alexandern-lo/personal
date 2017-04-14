using System;
using System.Globalization;

namespace StudioMobile
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string self, string substr)
        {
            if (self == null)
            {
                return false;
            }
            return self.IndexOf(substr, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }

        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ToShortNumber(this decimal number)
        {
            string returnString;

            if (number < 1000)
                returnString = Math.Round(number, MidpointRounding.AwayFromZero).ToString();
            else if (number >= 1000 && number < 100000)
                returnString = Math.Round(number / 1000, 1, MidpointRounding.AwayFromZero) + "k";
            else if (number >= 100000 && number < 1000000)
                returnString = Math.Round(number / 1000, MidpointRounding.AwayFromZero) + "k";
            else
                returnString = Math.Round(number / 1000000, MidpointRounding.AwayFromZero) + "m";

            return returnString;
        }

        public static string ToSpanString(this TimeSpan span)
        {
            var cult = CultureInfo.InvariantCulture;

            var prefix = string.Empty;
            if (span.Ticks == 0)
            {
                return "0 min";
            }
            if (span.Ticks < 0)
            {
                prefix = "-";
                span = span.Negate();
            }
            string s;

            if (span.TotalSeconds < 1)
            {
                s = span.TotalMilliseconds.ToString("0.# ms", cult);
            }
            else if (span.TotalMinutes < 1)
            {
                s = span.TotalSeconds.ToString("0.# sec", cult);
            }
            else if (span.TotalHours < 1)
            {
                s = span.TotalMinutes.ToString("0.# min", cult);
            }
            else if (span.TotalDays < 1)
            {
                s = span.TotalHours.ToString("0.# hr", cult);
            }
            else
            {
                var days = span.TotalDays.ToString("0.# day" + (span.TotalDays >= 2 ? "s" : null), cult);

                if (span.TotalDays > 365)
                {
                    var v = span.TotalDays / 365.25;
                    s = v.ToString("~0.# year" + (v >= 2 ? "s" : null), cult) + " (" + days + ")";
                }
                else
                {
                    s = days;
                }
            }

            return prefix + s;
        }
    }
}
