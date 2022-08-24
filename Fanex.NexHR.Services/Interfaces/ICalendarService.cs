namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.Calendar;
    using Models.Calendar;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICalendarService
    {
        Task<IList<WorkingSchedule>> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        Task<LeaveDaySummarySchedule> GetLeaveSchedule(GetLeaveScheduleCriteria criteria);
    }
}