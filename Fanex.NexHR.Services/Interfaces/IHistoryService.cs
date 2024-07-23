namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.History;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHistoryService
    {
        Task<IEnumerable<History>> GetAll(GetHistoryCriteria criteria);

        Task<History> GetById(GetHistoryByIdCriteria criteria);
    }
}