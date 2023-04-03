namespace Fanex.NexHR.Services
{
    using Criteria.Holiday;
    using Fanex.NexHR.Models.Calendar;
    using Interfaces;
    using Models.Holiday;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class HolidayService : IHolidayService
    {
        private readonly IBaseService baseService;

        public HolidayService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Holiday>> GetByYear(GetHolidayByYearCriteria criteria)
        {
            IEnumerable<Holiday> holidays = await baseService.GetDataAsync<Holiday>(criteria);
            return holidays;
        }
        public async Task<IEnumerable<WorkingSchedule>> GetWorkDateEmployeeCalendar(GetWorkDateEmployeeCalendarCriteria criteria)
        {
            IEnumerable<WorkingSchedule> holidays = await baseService.GetDataAsync<WorkingSchedule>(criteria);
            return holidays;
        }

        public async Task<Holiday> GetById(GetHolidayByIdCriteria criteria)
        {
            Holiday holiday = await baseService.GetDataEntityAsync<Holiday>(criteria);
            return holiday;
        }
    }
}