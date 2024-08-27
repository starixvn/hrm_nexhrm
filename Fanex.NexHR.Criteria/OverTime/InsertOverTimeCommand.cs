namespace Fanex.NexHR.Criteria.Overtime
{
    public class InsertOverTimeCommand : BaseActionOvertimeCommand
    {
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Overtime_Insert";

        public override bool IsValid() => base.IsValid() && CreatedBy > 0;
    }
}
