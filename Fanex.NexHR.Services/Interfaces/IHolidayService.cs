namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.Holiday;
    using Fanex.NexHR.Models.Calendar;
    using Models.Holiday;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHolidayService
    {
        Task<Holiday> GetById(GetHolidayByIdCriteria criteria);

        Task<IEnumerable<Holiday>> GetByYear(GetHolidayByYearCriteria criteria);

        Task<IEnumerable<WorkingSchedule>> GetWorkDateEmployeeCalendar(GetWorkDateEmployeeCalendarCriteria criteria);
    }
}