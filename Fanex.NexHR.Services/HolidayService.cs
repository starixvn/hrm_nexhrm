namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
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

        public IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria)
        {
            IList<Holiday> holidays = baseService.GetData<Holiday>(criteria);
            return holidays;
        }

        public Holiday GetById(GetHolidayByIdCriteria criteria)
        {
            Holiday holiday = baseService.GetDataEntity<Holiday>(criteria);
            return holiday;
        }
    }
}