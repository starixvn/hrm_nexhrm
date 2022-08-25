namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models;
    using Models.Overtime;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOvertimeRepository
    {
        Task<IList<Overtime>> GetOvertime(ICriteria criteria);

        Task<Overtime> GetOvertimeById(ICriteria criteria);

        Task<IList<EmployeeOvertime>> GetEmployeeOvertime(ICriteria criteria);

        Task<IList<OvertimeSummary>> GetOvertimeSummary(ICriteria criteria);

        Task<IList<OvertimeConversion>> GetConversions(ICriteria criteria);

        Task<IList<ReportOvertimeHRDetail>> GetReportOvertimeHRDetail(ICriteria criteria);
    }
}
