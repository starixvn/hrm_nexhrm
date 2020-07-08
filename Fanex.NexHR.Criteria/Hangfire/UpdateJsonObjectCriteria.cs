namespace Fanex.NexHR.Criteria.Hangfire
{
    using Fanex.Data.Repository;

    public class UpdateJsonObjectCriteria : CriteriaBase
    {
        public int CategoryId { get; set; }

        public string JsonObject { get; set; }

        public override string GetSettingKey() => "NexHangfire_UpdateJsonObject";

        public override bool IsValid() => CategoryId > 0;
    }
}