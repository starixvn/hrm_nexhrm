namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class HolidayService : BaseService, IHolidayService
    {
        public IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Holiday> holidays  = null;
            if (criteria.IsValid())
            {
                holidays = GetData<Holiday>(criteria).ToList();
            }

            return holidays ?? new List<Holiday>();
        }

        public Holiday GetById(GetHolidayByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Holiday holiday = null;
            if (criteria.IsValid())
            {
                holiday = GetDataEntity<Holiday>(criteria);
            }

            return holiday;
        }
    }
}