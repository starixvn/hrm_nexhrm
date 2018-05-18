namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.Calendar;

    public interface ICalendarRepository
    {
        IList<WorkingSchedule> GetWorkingSchedule(ICriteria criteria);

        IList<LeaveSchedule> GetLeaveSchedule(ICriteria criteria);
    }
}