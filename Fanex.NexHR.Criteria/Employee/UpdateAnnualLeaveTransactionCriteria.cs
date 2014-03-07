// -----------------------------------------------------------------------
// <copyright file="UpdateAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Update annual leave transaction criteria.
    /// </summary>
    public class UpdateAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_UpdateTransaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
