namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Calendar;
    using Models.Calendar;

    public interface ICalendarService
    {
        Task<IList<WorkingSchedule>> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        Task<LeaveDaySummarySchedule> GetLeaveSchedule(GetLeaveScheduleCriteria criteria);
    }
}