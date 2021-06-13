using System;
using System.Collections.Generic;

namespace WorkingTimes
{
    public interface IWorkingTimes
    {
        IEnumerable<WorkDay> GetWorkingDays(DateTime start, DateTime end);
    }
}
