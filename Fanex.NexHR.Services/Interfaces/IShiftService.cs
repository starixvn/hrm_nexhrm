namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;

    public interface IShiftService
    {
        IList<Shift> GetAll();

        Shift GetByDate(GetShiftByDateCriteria criteria);
    }
}
