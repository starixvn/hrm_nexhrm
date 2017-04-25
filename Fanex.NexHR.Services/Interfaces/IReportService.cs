namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.Report;
    using Models;
    using Models.Report;
    using System.Collections.Generic;

    public interface IReportService
    {
        IList<TimeRecord> GetTimeRecords(GetTimeRecordCriteria criteria);

        IList<LateInEarlyOutModel> GetLateInEarlyOut(GetLateInEarlyOutReportCriteria criteria);

        IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(GetLateInEarlyOutDetailReportCriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(GetRequestMonitorReportCriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(GetTotalWorkingHoursSummaryCriteria criteria);

        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceReportByTypeCriteria criteria);

        IList<LateInEarlyOutDiscipline> GetLateInEarlyOutDiscipline(GetLateInEarlyOutSummaryCriteria criteria);
    }
}