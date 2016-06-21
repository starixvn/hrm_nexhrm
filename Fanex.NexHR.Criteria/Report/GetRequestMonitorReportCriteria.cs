// -----------------------------------------------------------------------
// <copyright file="GetRequestMonitorReportCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) .. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get request monitor criteria.
    /// </summary>
    public class GetRequestMonitorReportCriteria : CriteriaBase
    {
        public int Role { get; set; }

        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int MaxRequest { get; set; }

        public override string GetSettingKey()
        {
            return "RequestMonitoring";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
