namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    public class EditConversationCriteria : BaseCommand
    {
        public int ConversationId { get; set; }

        public string SkypeConversationId { get; set; }

        public string ConversationName { get; set; }

        public string PermittedEmployeeIds { get; set; }

        public int EditedBy { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "Conversation_Update";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationName)
            && ConversationId > 0 && EditedBy > 0;
    }
}