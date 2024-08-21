namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class DeleteLeaveDeductionCommand : BaseCommand
    {
        public int LeaveDeductionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "LeaveDeduction_Delete";

        public override bool IsValid() => LeaveDeductionId > 0;
    }
}