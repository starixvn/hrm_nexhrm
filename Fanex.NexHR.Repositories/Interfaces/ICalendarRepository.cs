namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Calendar;

    public interface ICalendarRepository
    {
        IList<WorkingSchedule> GetWorkingSchedule(ICriteria criteria);
    }
}