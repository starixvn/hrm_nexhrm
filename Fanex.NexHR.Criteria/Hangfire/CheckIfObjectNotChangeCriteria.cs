namespace Fanex.NexHR.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class CheckIfObjectNotChangeCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public string JsonObject { get; set; }

        public override string GetSettingKey() => "MessageCategoryLog_CheckIfObjectNotChange";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(JsonObject) && CategoryId > 0;
    }
}