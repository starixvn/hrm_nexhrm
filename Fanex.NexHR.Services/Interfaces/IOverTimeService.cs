namespace Fanex.NexHR.Services.Interfaces
{
    using Models;
    using ViewModels;
    using System.Collections.Generic;

    public interface IOverTimeService
    {
        bool Insert(OverTimeViewModel model);

        bool InsertAndApprove(OverTimeViewModel model);

        IEnumerable<OverTime> GetSummaryReport(OverTimeSummaryFilterViewModel model);

        bool ApproveRequest(int id, int updatedUserId);

        bool DisapproveRequest(int id, int verifiedUserId);

        bool RejectRequest(int id, int rejectedUserId);

        OverTime Get(int id);

        bool Update(OverTimeViewModel model);
    }
}
