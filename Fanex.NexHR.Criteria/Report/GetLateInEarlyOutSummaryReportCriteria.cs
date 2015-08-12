// --------------------------------------------------------------------------
// <copyright file="GetLateInEarlyOutSummaryReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    /// <summary>
    /// GetLateInEarlyOutSummaryReport Criteria.
    /// </summary>
    public class GetLateInEarlyOutSummaryReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Report_GetLateInEarlyOutSummary";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
