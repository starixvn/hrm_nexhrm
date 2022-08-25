namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Holiday;
    using Interfaces;
    using Models.Holiday;

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

        public async Task<Holiday> GetById(GetHolidayByIdCriteria criteria)
        {
            Holiday holiday = await baseService.GetDataEntityAsync<Holiday>(criteria);
            return holiday;
        }
    }
}