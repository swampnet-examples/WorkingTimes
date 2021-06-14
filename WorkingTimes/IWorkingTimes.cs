using System;
using System.Collections.Generic;

namespace WorkingTimes
{
    /// <summary>
    /// It's down to the client to supply this, although we will supply some helpefr methods to help build it.
    /// Only the client knows what days/hours they work, and that could be different depending on department etc.
    /// </summary>
    public interface IWorkingTimes
    {
        /// <summary>
        /// Return working times between two dates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<WorkDay> GetWorkingTimes(DateTime start, DateTime end);
    }
}
