namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Shift;
    using Models;

    public interface IShiftService
    {
        IList<Shift> GetAll();

        Shift GetByDate(GetShiftByDateCriteria criteria);
    }
}
