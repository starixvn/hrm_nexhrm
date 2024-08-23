namespace Fanex.NexHR.Criteria.Position
{
    using Data.Repository;

    public class GetPositionByIdCriteria : CriteriaBase
    {
        public int PositionId { get; set; }

        public override bool IsValid() => PositionId > 0;

        public override string GetSettingKey() => "Position_GetById";
    }
}
