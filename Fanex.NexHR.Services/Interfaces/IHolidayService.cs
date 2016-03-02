namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models.Holiday;

    public interface IHolidayService
    {
        Holiday GetById(GetHolidayByIdCriteria criteria);

        IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria);
    }
}