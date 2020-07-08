namespace Fanex.NexHR.Criteria.Hangfire
{
    using Fanex.Data.Repository;

    public class GetJsonObjectByCategoryMessageCriteria : CriteriaBase
    {
        public int CategoryId { get; set; }

        public override string GetSettingKey() => "NexHangfire_GetJsonObjectByCategoryId";

        public override bool IsValid() => CategoryId > 0;
    }
}