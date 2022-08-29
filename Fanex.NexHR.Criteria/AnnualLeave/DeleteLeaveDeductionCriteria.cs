namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class DeleteLeaveDeductionCriteria : BaseCommand
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