namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class DeleteLeaveConversionCriteria : BaseCommand
    {
        public int LeaveConversionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveConversion_Delete";
        }

        public override bool IsValid()
        {
            return LeaveConversionId > 0;
        }
    }
}