namespace Fanex.NexHR.Criteria.Overtime
{
    public class ResetOvertimeCommand : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Overtime_ResetRequest";

        public override bool IsValid() => Id > 0 && ModifiedBy > 0;
    }
}