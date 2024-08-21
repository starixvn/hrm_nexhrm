namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class DeleteLeaveConversionCommand : BaseCommand
    {
        public int LeaveConversionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "LeaveConversion_Delete";

        public override bool IsValid() => LeaveConversionId > 0;
    }
}