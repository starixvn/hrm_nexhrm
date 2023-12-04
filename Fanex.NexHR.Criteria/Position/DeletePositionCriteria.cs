namespace Fanex.NexHR.Criteria.Position
{
    public class DeletePositionCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Position_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}