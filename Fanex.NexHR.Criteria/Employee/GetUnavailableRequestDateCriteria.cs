// -----------------------------------------------------------------------
// <copyright file="GetUnavailableRequestDateCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get unavailable request date criteria.
    /// </summary>
    public class GetUnavailableRequestDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetUnavailRequestDate";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
