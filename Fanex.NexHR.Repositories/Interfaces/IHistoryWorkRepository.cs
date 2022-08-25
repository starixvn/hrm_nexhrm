namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHistoryWorkRepository
    {
        Task<IList<HistoryWork>> GetHistoryWorkByEmpId(ICriteria criteria);

        HistoryWork EditHistoryWork(EditHistoryWorkCriteria criteria);

        Task<IList<HistoryWork>> GetFilterHistoryWork(ICriteria criteria);
    }
}
