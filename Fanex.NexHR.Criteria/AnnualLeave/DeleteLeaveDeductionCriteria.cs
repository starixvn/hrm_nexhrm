namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    public class DeleteLeaveDeductionCriteria : CriteriaBase
    {
        public int LeaveDeductionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Delete";
        }

        public override bool IsValid()
        {
            return LeaveDeductionId > 0;
        }
    }
}
