namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.HistoryWork;
    using Fanex.NexHR.ViewModels.Employee;
    using Models;

    public interface IHistoryWorkService
    {
        Task<IList<HistoryWork>> GetHistoryWorkByEmpId(GetHistoryWorkByEmpIdCriteria criteria);

        Task<WorkingHistoryViewModel> GetHistoryWorkInfo(EditHistoryWorkCriteria criteria);

        Task<IList<HistoryWork>> GetFilterHistoryWork(FilterHistoryWorkCriteria criteria);
    }
}
