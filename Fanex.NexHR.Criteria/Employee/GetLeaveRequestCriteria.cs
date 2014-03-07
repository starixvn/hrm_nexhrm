// -----------------------------------------------------------------------
// <copyright file="GetLeaveRequestCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get leave request criteria.
    /// </summary>
    public class GetLeaveRequestCriteria : CriteriaBase
    {
        public int Role { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? Status { get; set; }

        [SpParam(SqlParamName = "excludedEmpID")]
        public int? ExcludeEmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestByEmp_LeaveType";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
