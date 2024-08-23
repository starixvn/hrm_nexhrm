namespace Fanex.NexHR.Criteria.Promotion
{
    public class DeletePromotionCommand : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Promotion_Delete";

        public override bool IsValid() => Id > 0 && ModifiedBy > 0;
    }
}
