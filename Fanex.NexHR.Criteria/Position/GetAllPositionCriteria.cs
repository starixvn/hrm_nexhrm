namespace Fanex.NexHR.Criteria.Position
{
    using Data.Repository;

    public class GetAllPositionCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

        public override string GetSettingKey() => "Position_GetAll";

        public override bool IsValid() => true;
    }
}
