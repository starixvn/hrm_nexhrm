namespace Fanex.NexHR.Criteria.Hangfire
{
    using Fanex.Data.Repository;

    public class CheckIfObjectNotChangeCriteria : CriteriaBase
    {
        public int CategoryId { get; set; }

        public string JsonObject { get; set; }

        public override string GetSettingKey() => "NexHangfire_CheckIfObjectNotChange";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(JsonObject) && CategoryId > 0;
    }
}