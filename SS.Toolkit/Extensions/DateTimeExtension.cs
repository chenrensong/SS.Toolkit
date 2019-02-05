using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SS.Toolkit.Extensions
{
    /// <summary>
    /// 时间转换扩展
    /// </summary>
    public static class DateTimeExtension
    {

        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Regex _dateTime8601Regex = new Regex(
           @"(((?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2}))|((?<year>\d{4})(?<month>\d{2})(?<day>\d{2})))"
           + @"T"
           + @"(((?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}))|((?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})))"
           + @"(?<tz>$|Z|([+-]\d{2}:?(\d{2})?))");


        public static bool TryParseDateTime8601(this string date, out DateTime result)
        {
            result = DateTime.MinValue;
            Match m = _dateTime8601Regex.Match(date);
            if (m == null)
                return false;
            string normalized = m.Groups["year"].Value + m.Groups["month"].Value + m.Groups["day"].Value
                                + "T"
                                + m.Groups["hour"].Value + m.Groups["minute"].Value + m.Groups["second"].Value
                                + m.Groups["tz"].Value;
            var formats = new[] {
                "yyyyMMdd'T'HHmmss",
                "yyyyMMdd'T'HHmmss'Z'",
                "yyyyMMdd'T'HHmmsszzz",
                "yyyyMMdd'T'HHmmsszz"
            };

            try
            {
                result = DateTime.ParseExact(normalized, formats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 返回S
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long CurrentTimeSeconds(this DateTime d)
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalSeconds);
        }

        /// <summary>
        /// 返回毫秒
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long CurrentTimeMillis(this DateTime d)
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }

        public static DateTime ToDateTime(this long t)
        {
            if (t.ToString().Length == 10) // 秒
            {
                return Jan1st1970.AddSeconds(t).ToLocalTime();
            }
            else //毫秒
            {
                return Jan1st1970.AddMilliseconds(t).ToLocalTime();
            }
        }
    }
}
