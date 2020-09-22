namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models;
    using Models.Report;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(ICriteria criteria);

        IList<TimeRecord> GetTimeRecords(ICriteria criteria);

        IList<LateInEarlyOutModel> GetLateInEarlyOut(ICriteria criteria);

        IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(ICriteria criteria);

        IList<LateInEarlyOutDiscipline> GetLateInEarlyOutSummary(ICriteria criteria);

        IList<WorkingHoursStatistic> GetWorkingHoursStatistic(ICriteria criteria);

        IList<YearlyNexRequest> GetYearlyNexRequest(ICriteria criteria);
    }
}