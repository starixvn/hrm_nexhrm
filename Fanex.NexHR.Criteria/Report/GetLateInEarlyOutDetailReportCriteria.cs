// --------------------------------------------------------------------------
// <copyright file="GetLateInEarlyOutDetailReportCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.10">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Data.Repository;

    /// <summary>
    /// GetLateInEarlyOutDetailReport Criteria.
    /// </summary>
    public class GetLateInEarlyOutDetailReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public bool IncludeLeaveDeduction { get; set; }

        public override string GetSettingKey()
        {
            return "Report_GetLateInEarlyOutDetail";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
