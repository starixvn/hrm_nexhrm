namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Data.Repository;

    public class UpdateLeaveDeductionCriteria : CriteriaBase
    {
        public int LeaveDeductionId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Update";
        }

        public override bool IsValid()
        {
            return LeaveDeductionId > 0 && TotalDays > 0;
        }
    }
}
