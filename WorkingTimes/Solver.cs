using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimes
{
    public class Solver
    {
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
            
            double minutes = 0;

            // Get all valid working days between the two dates
            var workingTimes = workingtimes.GetWorkDays(start, end);
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

                    minutes += timeWorked.TotalMinutes;
                }
                day = day.AddDays(1);
            }

            return minutes;
        }
    }
}
