namespace Fanex.NexHR.Services.Interfaces
{
    using Models;
    using ViewModels;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;

    public interface IOvertimeService
    {
        CommandResult Insert(InsertOvertimeCriteria criteria);

        IEnumerable<Overtime> GetSummaryReport(GetSummaryCriteria criteria);

        CommandResult ApproveRequest(ApproveOvertimeRequestCriteria criteria);

        CommandResult DisapproveRequest(DisapproveOvertimeRequestCriteria criteria);

        CommandResult RejectRequest(UpdateStatusCriteria criteria);

        Overtime Get(GetByIdCriteria criteria);

        CommandResult Update(UpdateOvertimeCriteria criteria);
    }
}
