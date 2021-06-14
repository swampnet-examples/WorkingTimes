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
            var end = DateTime.Parse("2021-07-04 16:00");
            var actual = solver.Solve(workingTimes, start, end);

            // 20 Working days + 7 hours
            var expected = ((20*9) + 7) * 60;
            Assert.AreEqual(expected, actual);
        }


        class MockedWorkingTimes : IWorkingTimes
        {
            public IEnumerable<WorkDay> GetWorkingTimes(DateTime start, DateTime end)
            {
                var dayStart = TimeSpan.Parse("08:00:00");
                var dayEnd = TimeSpan.Parse("17:00:00");

                return Enumerable.Range(0, 1 + end.Subtract(start).Days)
                    .Select(offset => start.AddDays(offset))
                    .FilterWeekends()
                    .ToWorkDays(dayStart ,dayEnd)
                    ;
            }
        }
    }
}
