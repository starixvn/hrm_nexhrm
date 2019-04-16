namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetConversationsByIdCriteria : CriteriaBase
    {
        public int ConversationId { get; set; }

        public override string GetSettingKey() => "Conversation_GetById";

        public override bool IsValid() => ConversationId > 0;
    }
}