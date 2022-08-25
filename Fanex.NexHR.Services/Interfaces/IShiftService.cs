namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Shift;
    using Models;

    public interface IShiftService
    {
        Task<IEnumerable<Shift>> GetAll();

        Task<Shift> GetByDate(GetShiftByDateCriteria criteria);
    }
}
