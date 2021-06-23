using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-06-04 16:00");
            var actual = workingTimes.CalculateTimeSpan(start, end);
            var expected = TimeSpan.FromMinutes(6 * 60);
            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void Weekend()
        {
            var workingTimes = new MockedWorkingTimes();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-06-07 16:00");
            var actual = workingTimes.CalculateTimeSpan(start, end);
            var expected = TimeSpan.FromMinutes(900);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void StartOnNonWorkingDay()
        {
            var workingTimes = new MockedWorkingTimes();
            var start = DateTime.Parse("2021-06-05 10:00");    // Saturday (Non working day)
            var end = DateTime.Parse("2021-06-07 16:00");
            var actual = workingTimes.CalculateTimeSpan(start, end);
            var expected = TimeSpan.FromMinutes(8 * 60);                          // 08:00 - 16:00 on the Monday 2021-06-07
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void EndOnNonWorkingDay()
        {
            var workingTimes = new MockedWorkingTimes();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-06-05 16:00");   // Saturday (Non working day)
            var actual = workingTimes.CalculateTimeSpan(start, end);
            var expected = TimeSpan.FromMinutes(7 * 60);                          // 17:00 on the Friday 2021-06-04
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void CoupleOfWeeks()
        {
            var workingTimes = new MockedWorkingTimes();
            var start = DateTime.Parse("2021-06-04 10:00");
            var end = DateTime.Parse("2021-07-06 16:00");
            var actual = workingTimes.CalculateTimeSpan(start, end);

            // 20 Working days + 7 hours
            var expected = TimeSpan.FromMinutes(12240);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MidweekHoliday()
        {
            var workingTimes = new MockedWorkingTimes();
            var start = DateTime.Parse("2021-03-01 13:00"); // Monday
            var end = DateTime.Parse("2021-03-05 16:00");   // Friday
            var actual = workingTimes.CalculateTimeSpan(start, end);

            // Mon  1300-1700   4
            // Tue  0800-1700   9
            // Wed  HOLIDAY
            // Thu  HOLIDAY
            // Fri  0800-1600   8
            var expected = TimeSpan.FromMinutes(21 * 60);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void StartTime_Later_Than_WorkingDay_StartTime()
        {
            var workingTimes = new MockedWorkingTimes();

            var start = DateTime.Parse("2021-06-22 20:16"); // Tuesday, but outside working hours
            var end = DateTime.Parse("2021-06-23 08:40");   // Wednesday
            var actual = workingTimes.CalculateTimeSpan(start, end, "dept-02");

            // Should just be the 40 minutes on the Wednesday
            var expected = TimeSpan.FromMinutes(40);

            Assert.AreEqual(expected, actual);


            start = DateTime.Parse("2021-06-22 20:16"); // Tuesday, but outside working hours
            end = DateTime.Parse("2021-06-23 07:40");   // Wednesday, but outside working hours
            actual = workingTimes.CalculateTimeSpan(start, end, "dept-02");

            expected = TimeSpan.Zero;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EndTime_Earlier_Than_WorkingDay_StartTime()
        {
            var workingTimes = new MockedWorkingTimes();

            var start = DateTime.Parse("2021-06-22 20:16"); // Tuesday, but outside working hours
            var end = DateTime.Parse("2021-06-23 07:40");   // Wednesday, but outside working hours
            var actual = workingTimes.CalculateTimeSpan(start, end, "dept-02");

            var expected = TimeSpan.Zero;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void StartTime_Earlier_Than_WorkingDay_StartTime()
        {
            var workingTimes = new MockedWorkingTimes();

            var start = DateTime.Parse("2021-06-22 07:00"); // Tuesday, but outside working hours
            var end = DateTime.Parse("2021-06-23 08:40");   // Wednesday, but outside working hours
            var actual = workingTimes.CalculateTimeSpan(start, end, "dept-02");

            // One full working day + 40 minutes
            var expected = TimeSpan.FromMinutes((60 * 10) + 40);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void EndTime_Later_Than_WorkingDay_EndTime()
        {
            var workingTimes = new MockedWorkingTimes();

            var start = DateTime.Parse("2021-06-22 07:00"); // Tuesday, but outside working hours
            var end = DateTime.Parse("2021-06-23 20:40");   // Wednesday, but outside working hours
            var actual = workingTimes.CalculateTimeSpan(start, end, "dept-02");

            // Two full working days
            var expected = TimeSpan.FromMinutes(60 * 20);

            Assert.AreEqual(expected, actual);
        }


        class MockedWorkingTimes : IWorkingTimes
        {
            // Mock some working times
            // Normally this will be hitting the db / whatever to get working days / bank holidays whatever.
            // Just do weekends and a made up mid-week bank holiday in March
            //
            // 'context' here is if we need to pass in bespoke data that we might need to calculate this
            //  - eg, It's very possible that we might calculate working times differnt based on department (different
            //        hours almost certainly) this allows us to pass in anything we might need to determine that.
            //
            //      - Would we be better off passing in dayStart / dayEnd here?
            //          Torn. Implementing IWorkingTimes is responsibility of consumer so it's reasonable to expect
            //                it to implement business rules.
            public IEnumerable<WorkDay> GetWorkDays(DateTime start, DateTime end, object context)
            {
                // dept-01: 0800-1700
                // dept-02: 0600-2200
                var dept = (context as string) ?? "dept-01";

                var dayStart = dept == "dept-01" ? TimeSpan.Parse("08:00:00") : TimeSpan.Parse("08:00:00");
                var dayEnd = dept == "dept-01" ? TimeSpan.Parse("17:00:00") : TimeSpan.Parse("18:00:00");

                Debug.WriteLine($"Calculating work days for '{dept}' ({dayStart}-{dayEnd})");

                return Enumerable.Range(0, 1 + end.Date.Subtract(start.Date).Days)
                    .Select(offset => start.AddDays(offset))
                    .FilterWeekends()
                    .Filter(new[] {
                        DateTime.Parse("03 Mar 2021"), // Wednesday
                        DateTime.Parse("04 Mar 2021")  // Thursday 
                    })
                    .ToWorkDays(dayStart, dayEnd)
                    ;
            }
        }
    }
}
