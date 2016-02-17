namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.ViewModels;
    using Models;
    using System.Collections.Generic;

    public interface IOvertimeRepository
    {
        IEnumerable<Overtime> ReportSummary(ReportSummaryCriteria criteria);

        IEnumerable<Overtime> ReportDetail(ReportDetailCriteria criteria);

        OvertimeCreateViewModel Get(GetByIdCriteria criteria);

        IEnumerable<Overtime> GetMyReport(GetMyReportCriteria criteria);
    }
}
