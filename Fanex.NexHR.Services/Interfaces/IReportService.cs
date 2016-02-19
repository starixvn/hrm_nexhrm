namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;

    public interface IReportService
    {
        IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria);

        CommandResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria);

        IList<LateInEarlyOutModel> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria);

        IList<LateInEarlyOutByTeamModel> GetLateInEarlyOutByTeam(IList<LateInEarlyOutModel> lateInEarlyOuts, IList<Team> teams);

        IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria);

        IList<LeaveDeductionDetail> GetLeaveDeductionDetail(GetLeaveDeductionDetailCriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveSummaryReportCriteria criteria);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria);
    }
}