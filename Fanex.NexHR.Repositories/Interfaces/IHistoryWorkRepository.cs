namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IHistoryWorkRepository
    {
        IList<HistoryWork> GetHistoryWorkByEmpId(ICriteria criteria);

        HistoryWork EditHistoryWork(EditHistoryWorkCriteria criteria);

        IList<HistoryWork> GetFilterHistoryWork(ICriteria criteria);
    }
}
