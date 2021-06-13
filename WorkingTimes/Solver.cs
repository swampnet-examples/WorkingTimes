using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimes
{
    public interface IWorkingTimes
    {
        IEnumerable<WorkDay> GetWorkingDays(DateTime start, DateTime end);
    }


    /// <summary>
    /// Local time
    /// </summary>
    public class WorkDay
    {
        // Start of workingday
        public DateTime Start { get; set; }

        // End of working day
        public DateTime End { get; set; }
    }


    public class Solver
    {
        /*
         * Edge cases:
         * Starts on a non working day
         * Ends on a non working day
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">Local time start</param>
        /// <param name="end">Local time end</param>
        /// <returns></returns>
        public double Solve(IWorkingTimes workingtimes, DateTime start, DateTime end)
        {
            if(end < start)
            {
                throw new InvalidOperationException($"End date ({end}) must come after start date ({start})");
            }
            
            double minuets = 0;

            // Get all valid working days between the two dates
            var workingTimes = workingtimes.GetWorkingDays(start, end);
            var day = start.Date;

            while(day < end)
            {
                var workingDay = workingTimes.SingleOrDefault(wt => wt.Start.Date == day);
                if(workingDay != null)
                {
                    // Calculate the days start / end time
                    var startTime = day.Date == start.Date
                        ? start.TimeOfDay
                        : workingDay.Start.TimeOfDay;

                    var endTime = day.Date == end.Date
                        ? end.TimeOfDay
                        : workingDay.End.TimeOfDay;

                    var timeWorked = endTime - startTime;

                    Debug.WriteLine($"[{day:yyyy MMM dd}] {startTime} {endTime} ({timeWorked})");

                    minuets += timeWorked.TotalMinutes;
                }
                day = day.AddDays(1);
            }

            return minuets;
        }
    }
}
