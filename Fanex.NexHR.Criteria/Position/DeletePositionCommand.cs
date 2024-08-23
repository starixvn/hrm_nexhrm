namespace Fanex.NexHR.Criteria.Position
{
    public class DeletePositionCommand : BaseCommand
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Position_Delete";

        public override bool IsValid() => Id > 0;
    }
}