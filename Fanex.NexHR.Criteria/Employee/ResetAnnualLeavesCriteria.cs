// -----------------------------------------------------------------------
// <copyright file="ResetAnnualLeavesCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Reset annual leaves criteria.
    /// </summary>
    public class ResetAnnualLeavesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpValue")]
        public string EmployeeValue { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal ResetDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_ResetMultipleTransactions";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
