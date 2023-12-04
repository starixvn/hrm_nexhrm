namespace Fanex.NexHR.Criteria.Position
{
    public class InsertPositionCriteria : BaseCommand
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? DisplayOrder { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Position_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}