using System;
using System.Collections.Generic;

namespace WorkingTimes
{
    public interface IWorkingTimes
    {
        IEnumerable<WorkDay> GetWorkingTimes(DateTime start, DateTime end);
    }
}
