// -----------------------------------------------------------------------
// <copyright file="GetEmployeeAttendanceReportByTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) .. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using Core.Attributes;
    using Data.Repository;
    using System;

    public class GetEmployeeAttendanceReportByTypeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "fromtime")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "totime")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public bool ExcludeUserDelete { get; set; }

        [SpParam(SqlParamName = "Type")]
        public short ReportType { get; set; }

        [Output]
        public int? TotalItem { get; set; }

        public int? ItemsPerPage { get; set; }

        public int? PageIndex { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeAttendance_GetByType";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}