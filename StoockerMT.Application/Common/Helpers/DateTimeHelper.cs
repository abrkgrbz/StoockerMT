using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Application.Common.Extensions;

namespace StoockerMT.Application.Common.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetStartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime GetEndOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return GetStartOfWeek(date, startOfWeek).AddDays(6).EndOfDay();
        }

        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                yield return date;
            }
        }

        public static string GetQuarter(DateTime date)
        {
            return $"Q{(date.Month - 1) / 3 + 1} {date.Year}";
        }
    }
}
