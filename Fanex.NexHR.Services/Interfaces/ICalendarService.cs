namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Calendar;
    using Models.Calendar;

    public interface ICalendarService
    {
        IList<WorkingSchedule> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);
    }
}