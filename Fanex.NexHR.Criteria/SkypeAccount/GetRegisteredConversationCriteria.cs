namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Data.Repository;

    public class GetRegisteredConversationCriteria : CriteriaBase
    {
        public int CategoryId { get; set; }

        public override string GetSettingKey() => "Nexbot_GetRegisteredConversation";

        public override bool IsValid() => CategoryId > 0;
    }
}