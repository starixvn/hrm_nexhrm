namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class GetRegisteredConversationCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public override string GetSettingKey() => "Nexbot_GetRegisteredConversation";

        public override bool IsValid() => CategoryId > 0;
    }
}