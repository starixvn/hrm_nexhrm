namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class DeleteAnnualLeaveTransactionCriteria : BaseCommand
    {
        public int LeaveTransactionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_DeleteTransaction";
        }

        public override bool IsValid()
        {
            return LeaveTransactionId > 0;
        }
    }
}