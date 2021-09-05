using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkingTimes
{
    /// <summary>
    /// Some filter / projection extensions around DateTime (well, IEnumerable DateTime really)
    /// Try to adhere to deferred execution here, don't  actually materialise the dates unless we really have to.
    /// </summary>
    public static partial class DateTimeExtensions
    {
        public static IEnumerable<DateTime> FilterWeekends(this IEnumerable<DateTime> source)
        {
            return source.Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
        }


        public static IEnumerable<DateTime> Filter(this IEnumerable<DateTime> source, IEnumerable<DateTime> filter)
        {
            return source.Where(d => !filter.Any(f => f.Date == d.Date));
        }


        public static IEnumerable<WorkDay> ToWorkDays(this IEnumerable<DateTime> source, TimeSpan dayStart, TimeSpan dayEnd)
        {
            return source.Select(r => new WorkDay()
            {
                Start = new DateTime(r.Year, r.Month, r.Day, dayStart.Hours, dayStart.Minutes, 0),
                End = new DateTime(r.Year, r.Month, r.Day, dayEnd.Hours, dayEnd.Minutes, 0)
            });
        }
    }
}
