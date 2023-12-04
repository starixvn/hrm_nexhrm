namespace Fanex.NexHR.Criteria.Position
{
    public class UpdatePositionCriteria : BaseCommand
    {
        public int PositionId { get; set; }

        public string PositionCode { get; set; }

        public string PositionName { get; set; }

        public bool IsActive { get; set; }

        public int? DisplayOrder { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Position_Update";
        }

        public override bool IsValid()
        {
            return PositionId > 0 && !string.IsNullOrWhiteSpace(PositionCode) && !string.IsNullOrWhiteSpace(PositionName);
        }
    }
}