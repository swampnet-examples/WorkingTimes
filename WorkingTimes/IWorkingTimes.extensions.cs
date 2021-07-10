using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimes
{
    public static class IWorkingTimesExtensions
    {
        /// <summary>
        /// Calclulate the amount of time between to given points, taking into account the values in IWorkingTimes
        /// </summary>
        public static TimeSpan CalculateTimeSpan(this IWorkingTimes wt, DateTime start, DateTime end, object context = null)
        {
            // If end/start, swap 'em over rather than throwing an exception.
            if (end < start)
            {
                var tmp = start;
                start = end;
                end = tmp;
                //throw new InvalidOperationException($"End date ({end}) must come after start date ({start})");
            }

            var ts = TimeSpan.Zero;

            // Get all valid working days between the two dates
            var workingTimes = wt.GetWorkDays(start, end, context);
            var day = start.Date;

            while (day < end)
            {
                var workingDay = workingTimes.SingleOrDefault(t => t.Start.Date == day);
                if (workingDay != null)
                {
                    // Calculate the days start / end time
                    var startTime = day.Date == start.Date
                        // Start day. Clamp start time
                        ? start.TimeOfDay > workingDay.End.TimeOfDay
                            ? workingDay.End.TimeOfDay
                            : start.TimeOfDay < workingDay.Start.TimeOfDay
                                ? workingDay.Start.TimeOfDay
                                : start.TimeOfDay

                        : workingDay.Start.TimeOfDay;

                    var endTime = day.Date == end.Date
                        // End day. Clamp end time
                        ? end.TimeOfDay < workingDay.Start.TimeOfDay
                            ? workingDay.Start.TimeOfDay
                            : end.TimeOfDay > workingDay.End.TimeOfDay
                                ? workingDay.End.TimeOfDay
                                : end.TimeOfDay

                        : workingDay.End.TimeOfDay;

                    var timeWorked = endTime - startTime;

                    ts += timeWorked;

                    Debug.WriteLine($"[{day:yyyy MMM dd}] {startTime} {endTime} (day: {timeWorked} - total: {ts})");
                }
                day = day.AddDays(1);
            }

            return ts;
        }
    }
}
