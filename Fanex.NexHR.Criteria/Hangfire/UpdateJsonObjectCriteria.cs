namespace Fanex.NexHR.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class UpdateJsonObjectCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public string JsonObject { get; set; }

        public override string GetSettingKey() => "MessageCategoryLog_UpdateJsonObject";

        public override bool IsValid() => CategoryId > 0;
    }
}