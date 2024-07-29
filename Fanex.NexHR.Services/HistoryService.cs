namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.History;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class HistoryService : IHistoryService
    {
        private readonly IBaseService baseService;

        public HistoryService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<IEnumerable<History>> GetAll(GetHistoryCriteria criteria)
        {
            var history = await baseService.GetDataAsync<History>(criteria);
            return history.ToList();
        }

        public async Task<History> GetById(GetHistoryByIdCriteria criteria)
        {
            var history = await baseService.GetDataEntityAsync<History>(criteria);
            return history;
        }

        public async Task<IEnumerable<History>> GetByEmpId(GetHistoryByEmpIdCriteria criteria)
        {
            var history = await baseService.GetDataAsync<History>(criteria);
            return history;
        }
    }
}
