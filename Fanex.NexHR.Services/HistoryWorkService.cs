namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Employee;

    public class HistoryWorkService : IHistoryWorkService
    {
        private readonly IHistoryWorkRepository historyWorkRepository;
        private readonly IBaseService baseService;

        public HistoryWorkService(IHistoryWorkRepository historyWorkRepository, IBaseService baseService = null)
        {
            this.historyWorkRepository = historyWorkRepository;
            this.baseService = baseService;
        }

        public async Task<IList<HistoryWork>> GetHistoryWorkByEmpId(GetHistoryWorkByEmpIdCriteria criteria)
        {
            IList<HistoryWork> historyWork = null;
            if (criteria != null && criteria.IsValid())
            {
                historyWork = await historyWorkRepository.GetHistoryWorkByEmpId(criteria);
            }

            return historyWork ?? new List<HistoryWork>();
        }

        public async Task<WorkingHistoryViewModel> GetHistoryWorkInfo(EditHistoryWorkCriteria criteria)
        {
            WorkingHistoryViewModel historyWork = await baseService.GetDataEntityAsync<WorkingHistoryViewModel>(criteria);
            return historyWork;
        }

        public async Task<IList<HistoryWork>> GetFilterHistoryWork(FilterHistoryWorkCriteria criteria)
        {
            IList<HistoryWork> filterHistory = null;
            if (criteria != null && criteria.IsValid())
            {
                filterHistory = await historyWorkRepository.GetFilterHistoryWork(criteria);
            }

            return filterHistory ?? new List<HistoryWork>();
        }
    }
}