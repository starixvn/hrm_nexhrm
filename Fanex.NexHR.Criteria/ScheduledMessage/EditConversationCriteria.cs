namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class EditConversationCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string ConversationId { get; set; }

        public string ConversationName { get; set; }

        public string PermittedEmployeeIds { get; set; }

        public int EditedBy { get; set; }

        public override string GetSettingKey() => "Conversation_Update";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId) && !string.IsNullOrWhiteSpace(ConversationName)
            && Id > 0 && EditedBy > 0;
    }
}