// --------------------------------------------------------------------------
// <copyright file="IReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.7">Apply Nap Framework</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria);

        IList<TotalWorkingHours> GetTotalWorkingHours(ICriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(ICriteria criteria);

        IList<LateInEarlyOutSummary> GetLateInEarlyOutSummary(ICriteria criteria);

        IList<LateInEarlyOutDetail> GetLateInEarlyOutDetail(ICriteria criteria);

        IList<LeaveDeductionDetail> GetLeaveDeductionDetail(ICriteria criteria);
    }
}
