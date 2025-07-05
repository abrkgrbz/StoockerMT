using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Date;
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddMilliseconds(-1);
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return $"{timeSpan.Seconds} seconds ago";

            if (timeSpan <= TimeSpan.FromMinutes(60))
                return timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} minutes ago" : "a minute ago";

            if (timeSpan <= TimeSpan.FromHours(24))
                return timeSpan.Hours > 1 ? $"{timeSpan.Hours} hours ago" : "an hour ago";

            if (timeSpan <= TimeSpan.FromDays(30))
                return timeSpan.Days > 1 ? $"{timeSpan.Days} days ago" : "yesterday";

            if (timeSpan <= TimeSpan.FromDays(365))
                return timeSpan.Days > 30 ? $"{timeSpan.Days / 30} months ago" : "a month ago";

            return timeSpan.Days > 365 ? $"{timeSpan.Days / 365} years ago" : "a year ago";
        }
    }
}
