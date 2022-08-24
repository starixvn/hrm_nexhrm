namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.Report;
    using Models;
    using Models.Report;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IReportService
    {
        Task<IList<TimeRecord>> GetTimeRecords(GetTimeRecordCriteria criteria);

        bool GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria);

        Task<IList<LateInEarlyOutModel>> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria);

        Task<IList<LateInEarlyOutDetail>> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria);

        IList<LateInEarlyOutDiscipline> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria);

        IList<WorkingHoursStatistic> GetWorkingHoursStatistic(GetWorkingHoursStatisticCriteria criteria);

        IList<HeadCount> GetTeamHeadCountInYear(GetHeadCountByYearCriteria criteria);

        IList<YearlyNexRequest> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria);
    }
}