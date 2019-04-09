namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class CreateConversationCriteria : CriteriaBase
    {
        public string ConversationId { get; set; }

        public string ConversationName { get; set; }

        public int CreatedBy { get; set; }

        public string PermittedEmployeeIds { get; set; }

        public override string GetSettingKey() => "Conversation_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId) && !string.IsNullOrWhiteSpace(ConversationName) && CreatedBy > 0;
    }
}