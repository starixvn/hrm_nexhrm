namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models;
    using Models.Report;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IReportRepository
    {
        Task<IList<EmployeeAttendanceModel>> GetEmployeeAttendanceReport(ICriteria criteria);

        Task<IList<RequestsMonitoring>> GetRequestMonitoring(ICriteria criteria);

        Task<IList<TotalWorkingHours>> GetTotalWorkingHours(ICriteria criteria);

        Task<IList<TimeRecord>> GetTimeRecords(ICriteria criteria);

        Task<IList<LateInEarlyOutModel>> GetLateInEarlyOut(ICriteria criteria);

        Task<IList<LateInEarlyOutDetail>> GetLateInEarlyOutDetail(ICriteria criteria);

        Task<IList<LateInEarlyOutDiscipline>> GetLateInEarlyOutSummary(ICriteria criteria);

        Task<IList<WorkingHoursStatistic>> GetWorkingHoursStatistic(ICriteria criteria);

        Task<IList<YearlyNexRequest>> GetYearlyNexRequest(ICriteria criteria);
    }
}