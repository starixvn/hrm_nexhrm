namespace Fanex.NexHR.Criteria.Overtime
{
    public class DeleteOvertimeCommand : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedId { get; set; }

        public override string GetSettingKey() => "Overtime_Delete";

        public override bool IsValid() => Id > 0 && ModifiedId > 0;
    }
}