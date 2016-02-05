namespace Fanex.NexHR.Services.Interfaces
{
    using Models;
    using ViewModels;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;

    public interface IOvertimeService
    {
        CommandResult Insert(InsertOvertimeCriteria criteria);

        IEnumerable<Overtime> ReportSummary(ReportSummaryCriteria criteria);

        IEnumerable<Overtime> ReportDetail(ReportDetailCriteria criteria);

        CommandResult ApproveRequest(ApproveOvertimeRequestCriteria criteria);

        CommandResult DisapproveRequest(DisapproveOvertimeRequestCriteria criteria);

        CommandResult RejectRequest(RejectRequestCriteria criteria);

        OvertimeCreateViewModel Get(GetByIdCriteria criteria);

        CommandResult Update(UpdateOvertimeCriteria criteria);
    }
}
