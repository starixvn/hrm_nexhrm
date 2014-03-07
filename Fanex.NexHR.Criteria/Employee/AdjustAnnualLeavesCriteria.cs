// -----------------------------------------------------------------------
// <copyright file="AdjustAnnualLeavesCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Adjust annual leaves criteria.
    /// </summary>
    public class AdjustAnnualLeavesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpIds")]
        public string EmployeeId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductsDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_InsertMultipleTransactions";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
