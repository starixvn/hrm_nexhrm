namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeave;
    using Fanex.NexHR.Models.Report;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(ICriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(ICriteria criteria);

        IList<LateInEarlyOutModel> GetLateInEarlyOut(ICriteria criteria);

        IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(ICriteria criteria);

        IList<LeaveDeductionDetail> GetLeaveDeductionDetail(ICriteria criteria);

        IList<LateInEarlyOutSummaryModel> GetLateInEarlyOutSummary(ICriteria criteria);
    }
}