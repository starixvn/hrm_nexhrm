namespace Fanex.NexHR.Services.Interfaces
{
    using Models;
    using ViewModels;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.OverTime;

    public interface IOverTimeService
    {
        CommandResult Insert(InsertOverTimeCriteria criteria);

        IEnumerable<OverTime> GetSummaryReport(GetSummaryCriteria criteria);

        CommandResult ApproveRequest(ApproveOverTimeRequestCriteria criteria);

        CommandResult DisapproveRequest(DisapproveOverTimeRequestCriteria criteria);

        CommandResult RejectRequest(UpdateStatusCriteria criteria);

        OverTime Get(GetByIdCriteria criteria);

        CommandResult Update(UpdateOverTimeCriteria criteria);
    }
}
