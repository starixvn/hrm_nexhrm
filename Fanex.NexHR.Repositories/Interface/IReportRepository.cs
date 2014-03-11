// --------------------------------------------------------------------------
// <copyright file="IReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria);

        IList<TotalWorkingHours> GetWorkingHoursList(ICriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria);
    }
}
