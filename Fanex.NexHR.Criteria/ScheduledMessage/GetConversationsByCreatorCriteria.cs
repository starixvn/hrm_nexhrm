namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetConversationsByCreatorCriteria : CriteriaBase
    {
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Conversation_GetByCreator";

        public override bool IsValid() => CreatedBy > 0;
    }
}