namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class HistoryWorkService : IHistoryWorkService
    {
        private readonly IBaseService baseService;

        public HistoryWorkService(IBaseService baseService = null)
        {
            this.baseService = baseService;
        }

        public async Task<HistoryWork> GetById(GetHistoryWorkByIdCriteria criteria)
        {
            var transfers = await baseService.GetDataEntityAsync<HistoryWork>(criteria);
            return transfers;
        }


        public async Task<IEnumerable<HistoryWork>> GetAll(FilterHistoryWorkCriteria criteria)
        {
            var transfers = await baseService.GetDataAsync<HistoryWork>(criteria);

            return transfers;
        }
    }
}