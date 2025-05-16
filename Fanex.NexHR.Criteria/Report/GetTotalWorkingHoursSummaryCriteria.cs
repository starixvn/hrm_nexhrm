// -----------------------------------------------------------------------
// <copyright file="GetTotalWorkingHoursSummaryCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) .. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Data;
    using Data.Repository;

    /// <summary>
    /// Get total working hours summary criteria.
    /// </summary>
    public class GetTotalWorkingHoursSummaryCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int? DepartmentId { get; set; }

        public override string GetSettingKey()
        {
            return "Report_WorkingHoursSummary";
        }

        public override bool IsValid()
        {
             return FromDate <= ToDate;
        }
    }
}
