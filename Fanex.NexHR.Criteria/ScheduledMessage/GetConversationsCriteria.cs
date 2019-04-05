namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetConversationsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Conversation_Get";

        public override bool IsValid() => true;
    }
}