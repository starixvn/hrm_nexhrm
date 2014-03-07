// -----------------------------------------------------------------------
// <copyright file="GetAllRequestByDateCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get all request by date criteria.
    /// </summary>
    public class GetAllRequestByDateCriteria : CriteriaBase
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
            return "WorkingTimeRequest_GetRequestByDate_AllType";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
