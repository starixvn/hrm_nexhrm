namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

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
