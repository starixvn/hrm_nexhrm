namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class HolidayService : BaseService, IHolidayService
    {
        public IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria)
        {
            IList<Holiday> holidays = GetData<Holiday>(criteria);
            return holidays;
        }

        public Holiday GetById(GetHolidayByIdCriteria criteria)
        {
            Holiday holiday = GetDataEntity<Holiday>(criteria);
            return holiday;
        }
    }
}