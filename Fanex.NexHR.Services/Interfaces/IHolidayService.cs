namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Holiday;
    using Models.Holiday;

    public interface IHolidayService
    {
        Holiday GetById(GetHolidayByIdCriteria criteria);

        IList<Holiday> GetByYear(GetHolidayByYearCriteria criteria);
    }
}