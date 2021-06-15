using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkingTimes;

namespace WorkingTimes.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void SameDay()
        {
            var workingTimes = new MockedWorkingTimes();
            var solver = new Solver();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-06-04 16:00");
            var actual = solver.Solve(workingTimes, start, end);
            var expected = 6*60;
            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void Weekend()
        {
            var workingTimes = new MockedWorkingTimes();
            var solver = new Solver();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-06-07 16:00");
            var actual = solver.Solve(workingTimes, start, end);
            var expected = 900;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void StartOnNonWorkingDay()
        {
            var workingTimes = new MockedWorkingTimes();
            var solver = new Solver();
            var start = DateTime.Parse("2021-06-05 10:00");    // Saturday (Non working day)
            var end = DateTime.Parse("2021-06-07 16:00");
            var actual = solver.Solve(workingTimes, start, end);
            var expected = 8 * 60;                          // 08:00 - 16:00 on the Monday 2021-06-07
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void EndOnNonWorkingDay()
        {
            var workingTimes = new MockedWorkingTimes();
            var solver = new Solver();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-06-05 16:00");   // Saturday (Non working day)
            var actual = solver.Solve(workingTimes, start, end);
            var expected = 7 * 60;                          // 17:00 on the Friday 2021-06-04
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void CoupleOfWeeks()
        {
            var workingTimes = new MockedWorkingTimes();
            var solver = new Solver();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-07-06 16:00");
            var actual = solver.Solve(workingTimes, start, end);

            // 20 Working days + 7 hours
            var expected = 12240;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MidweekHoliday()
        {
            var workingTimes = new MockedWorkingTimes();
            var solver = new Solver();
            var start = DateTime.Parse("2021-03-01 13:00"); // Monday
            var end = DateTime.Parse("2021-03-05 16:00");   // Friday
            var actual = solver.Solve(workingTimes, start, end);

            // Mon  1300-1700   4
            // Tue  0800-1700   9
            // Wed  HOLIDAY
            // Thu  HOLIDAY
            // Fri  0800-1600   8
            var expected = 21*60;
            Assert.AreEqual(expected, actual);
        }


        class MockedWorkingTimes : IWorkingTimes
        {
            // Mock some working times
            // Normally this will be hitting the db / whatever to get working days / bank holidays whatever.
            // Just do weekends and a made up mid-week bank holiday in March
            public IEnumerable<WorkDay> GetWorkDays(DateTime start, DateTime end)
            {
                var dayStart = TimeSpan.Parse("08:00:00");
                var dayEnd = TimeSpan.Parse("17:00:00");

                return Enumerable.Range(0, 1 + end.Subtract(start).Days)
                    .Select(offset => start.AddDays(offset))
                    .FilterWeekends()
                    .Filter(new[] {
                        DateTime.Parse("03 Mar 2021"), // Wednesday
                        DateTime.Parse("04 Mar 2021")  // Thursday 
                    })
                    .ToWorkDays(dayStart ,dayEnd)
                    ;
            }
        }
    }
}
