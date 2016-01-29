namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;
    using System.Collections.Generic;

    public interface IOverTimeRepository
    {
        bool Insert(ICriteria createCriteria);

        IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model);

        OverTime Get(int id);

        bool Update(OverTimeViewModel model);

        bool UpdateStatus(int id, RequestStatuse status, int rejectedUserId);

        bool ApproveRequest(int id, int userId);

        bool DisapproveRequest(int id, int userId);
    }
}
