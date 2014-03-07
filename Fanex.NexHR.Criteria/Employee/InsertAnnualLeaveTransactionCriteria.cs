// -----------------------------------------------------------------------
// <copyright file="InsertAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Insert annual leave transaction criteria.
    /// </summary>
    public class InsertAnnualLeaveTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public int? RequestId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_InsertTransaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
