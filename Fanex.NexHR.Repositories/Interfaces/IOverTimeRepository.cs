namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models;

    public interface IOvertimeRepository
    {
        IEnumerable<Overtime> ReportSummary(ICriteria criteria);

        IEnumerable<Overtime> ReportDetail(ICriteria criteria);

        Overtime Get(ICriteria criteria);

        IEnumerable<Overtime> GetMyReport(ICriteria criteria);
    }
}
