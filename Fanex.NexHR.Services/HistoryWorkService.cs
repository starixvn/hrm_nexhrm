namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Employee;
    using System.Collections.Generic;

    public class HistoryWorkService : BaseService, IHistoryWorkService
    {
        private readonly IHistoryWorkRepository historyWorkRepository;

        public HistoryWorkService(IHistoryWorkRepository historyWorkRepository = null)
        {
            this.historyWorkRepository = historyWorkRepository ?? new HistoryWorkRepository();
        }

        public IList<HistoryWork> GetHistoryWorkByEmpId(GetHistoryWorkByEmpIdCriteria criteria)
        {
            IList<HistoryWork> historyWork = null;
            if (criteria != null && criteria.IsValid())
            {
                historyWork = historyWorkRepository.GetHistoryWorkByEmpId(criteria);
            }

            return historyWork ?? new List<HistoryWork>();
        }

        public WorkingHistoryViewModel GetHistoryWorkInfo(EditHistoryWorkCriteria criteria)
        {
            WorkingHistoryViewModel historyWork = GetDataEntity<WorkingHistoryViewModel>(criteria);
            return historyWork;
        }

        public IList<HistoryWork> GetFilterHistoryWork(FilterHistoryWorkCriteria criteria)
        {
            IList<HistoryWork> filterHistory = null;
            if (criteria != null && criteria.IsValid())
            {
                filterHistory = historyWorkRepository.GetFilterHistoryWork(criteria);
            }

            return filterHistory ?? new List<HistoryWork>();
        }
    }
}
