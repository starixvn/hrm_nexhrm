namespace Fanex.NexHR.Criteria.Overtime
{
    public class UpdateOverTimeCommand : BaseActionOvertimeCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }
        
        public override string GetSettingKey() => "Overtime_Update";

        public override bool IsValid() => base.IsValid() && Id > 0 && ModifiedBy > 0;
    }
}
