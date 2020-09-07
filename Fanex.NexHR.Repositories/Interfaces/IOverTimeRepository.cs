namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models;
    using Models.Overtime;

    public interface IOvertimeRepository
    {
        IList<Overtime> GetOvertime(ICriteria criteria);

        Overtime GetOvertimeById(ICriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(ICriteria criteria);

        IList<OvertimeSummary> GetOvertimeSummary(ICriteria criteria);

        IList<OvertimeConversion> GetConversions(ICriteria criteria);

        IList<ReportOvertimeHRDetail> GetReportOvertimeHRDetail(ICriteria criteria);
    }
}
