namespace Fanex.NexHR.Criteria.History
{
    using Fanex.Data.Repository;

    public class GetHistoryByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Transfer_GetById";

        public override bool IsValid() => Id > 0;
    }
}
