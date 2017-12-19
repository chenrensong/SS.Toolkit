using System;

namespace SS.Toolkit.Extensions
{
    /// <summary>
    /// 时间转换扩展
    /// </summary>
    public static class DateTimeExtension
    {

        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis(this DateTime d)
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }

        public static DateTime ToDateTime(this long t)
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (t.ToString().Length == 10)
            {
                return time.AddSeconds(t).ToLocalTime();
            }
            else
            {
                return time.AddMilliseconds(t).ToLocalTime();
            }
        }
    }
}
