namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Criteria.Calendar;
    using Models.Calendar;

    public interface ICalendarService
    {
        Task<LeaveDaySummarySchedule> GetLeaveSchedule(GetLeaveScheduleCriteria criteria);
    }
}