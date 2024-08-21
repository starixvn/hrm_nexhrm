namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class DeleteAnnualLeaveTransactionCommand : BaseCommand
    {
        public int LeaveTransactionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_DeleteTransaction";

        public override bool IsValid() => LeaveTransactionId > 0;
    }
}