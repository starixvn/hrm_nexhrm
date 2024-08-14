namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Report;
    using Models;
    using Models.Report;

    public interface IReportService
    {
        Task<IList<TimeRecord>> GetTimeRecords(GetTimeRecordCriteria criteria);

        Task<bool> GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria);

        Task<IEnumerable<LateInEarlyOutSummary>> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria);

        Task<IList<LateInEarlyOutDetail>> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria);

        Task<IList<RequestsMonitoring>> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria);

        Task<IList<TotalWorkingHours>> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria);

        Task<IList<EmployeeAttendanceModel>> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria);

        Task<IList<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria);

        Task<IList<WorkingHoursStatistic>> GetWorkingHoursStatistic(GetWorkingHoursStatisticCriteria criteria);

        Task<IEnumerable<HeadCount>> GetTeamHeadCountInYear(GetHeadCountByYearCriteria criteria);

        Task<IList<YearlyNexRequest>> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria);
    }
}