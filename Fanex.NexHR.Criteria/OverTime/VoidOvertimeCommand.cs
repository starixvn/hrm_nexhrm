namespace Fanex.NexHR.Criteria.Overtime
{
    public class VoidOvertimeCommand : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Overtime_VoidRequest";

        public override bool IsValid() => Id > 0 && ModifiedBy > 0;
    }
}