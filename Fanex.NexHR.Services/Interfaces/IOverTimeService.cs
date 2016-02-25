namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;
    using Models;
    using ViewModels;

    public interface IOvertimeService
    {
        IEnumerable<Overtime> ReportSummary(ReportSummaryCriteria criteria);

        IEnumerable<Overtime> ReportDetail(ReportDetailCriteria criteria);

        Overtime Get(GetByIdCriteria criteria);

        IEnumerable<Overtime> GetMyReport(GetMyReportCriteria criteria);
    }
}
