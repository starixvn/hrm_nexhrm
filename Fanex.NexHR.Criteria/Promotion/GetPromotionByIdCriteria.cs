namespace Fanex.NexHR.Criteria.Promotion
{
    using Fanex.Data.Repository;

    public class GetPromotionByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Promotion_GetById";

        public override bool IsValid() => Id > 0;
    }
}
