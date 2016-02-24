namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;

    public interface IHolidayService
    {
        bool Delete(int holidayId, int modifiedBy);

        Holiday GetById(int id);

        IList<Holiday> GetByYear(int year);

        int Insert(InsertNewHolidayCriteria criteria);

        bool Update(UpdateHolidayCriteria criteria);
    }
}