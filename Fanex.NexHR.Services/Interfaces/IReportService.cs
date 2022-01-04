namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Report;
    using Models;
    using Models.Report;

    public interface IReportService
    {
        IList<TimeRecord> GetTimeRecords(GetTimeRecordCriteria criteria);

        bool GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria);

        bool GetIsPrivilegeEditCheckInOut(GetPrivilegeEditCheckInOutRecordCriteria criteria);

        IList<LateInEarlyOutModel> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria);

        IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria);

        IList<LateInEarlyOutDiscipline> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria);

        IList<WorkingHoursStatistic> GetWorkingHoursStatistic(GetWorkingHoursStatisticCriteria criteria);

        IList<HeadCount> GetTeamHeadCountInYear(GetHeadCountByYearCriteria criteria);

        IList<YearlyNexRequest> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria);
    }
}