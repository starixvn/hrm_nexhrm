namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.HistoryWork;
    using Fanex.NexHR.ViewModels.Employee;
    using Models;
    using System.Collections.Generic;

    public interface IHistoryWorkService
    {
        IList<HistoryWork> GetHistoryWorkByEmpId(GetHistoryWorkByEmpIdCriteria criteria);

        WorkingHistoryViewModel GetHistoryWorkInfo(EditHistoryWorkCriteria criteria);

        IList<HistoryWork> GetFilterHistoryWork(FilterHistoryWorkCriteria criteria);
    }
}
