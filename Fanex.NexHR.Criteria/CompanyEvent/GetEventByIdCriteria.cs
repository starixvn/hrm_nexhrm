namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using Data.Repository;

    public class GetEventByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid() => Id > 0;

        public override string GetSettingKey() => "CompanyEvent_GetEventById";
    }
}
