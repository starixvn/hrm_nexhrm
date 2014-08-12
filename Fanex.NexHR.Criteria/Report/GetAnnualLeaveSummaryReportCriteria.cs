// -----------------------------------------------------------------------
// <copyright file="GetAnnualLeaveSummaryReportCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get annual leave summary criteria.
    /// </summary>
    public class GetAnnualLeaveSummaryReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Report_LeaveSummary";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
