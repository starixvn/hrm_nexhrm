namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models.Calendar;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICalendarRepository
    {
        Task<IList<WorkingSchedule>> GetWorkingSchedule(ICriteria criteria);

        Task<IList<LeaveSchedule>> GetLeaveSchedule(ICriteria criteria);
    }
}