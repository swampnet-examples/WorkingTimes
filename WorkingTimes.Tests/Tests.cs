using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
                var workDays = new List<WorkDay>();

                var day = start.Date;

                while(day < end)
                {
                    if(day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workDays.Add(new WorkDay()
                        {
                            Start = new DateTime(day.Year, day.Month, day.Day, 08, 0, 0),
                            End = new DateTime(day.Year, day.Month, day.Day, 17, 0, 0)
                        });
                    }

                    day = day.AddDays(1);
                }

                return workDays;
            }
        }
    }
}
