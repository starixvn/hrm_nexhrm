// -----------------------------------------------------------------------
// <copyright file="GetEmployeeAttendanceReportByTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get employee attendance report by type criteria.
    /// </summary>
    public class GetEmployeeAttendanceReportByTypeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "fromtime")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "totime")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int ExcludeUserDelete { get; set; }

        public short Type { get; set; }

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
