namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.HistoryWork;
    using Models;

    public interface IHistoryWorkService
    {
        Task<IEnumerable<HistoryWork>> GetAll(FilterHistoryWorkCriteria criteria);

        Task<HistoryWork> GetById(GetHistoryWorkByIdCriteria criteria);
    }
}
