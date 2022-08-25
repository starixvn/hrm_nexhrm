namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Holiday;
    using Models.Holiday;

    public interface IHolidayService
    {
        Task<Holiday> GetById(GetHolidayByIdCriteria criteria);

        Task<IEnumerable<Holiday>> GetByYear(GetHolidayByYearCriteria criteria);
    }
}