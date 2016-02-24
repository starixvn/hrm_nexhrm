namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.Calendar;

    public interface ICalendarService
    {
        IList<WorkingSchedule> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);
    }
}