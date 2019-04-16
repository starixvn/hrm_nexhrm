namespace Fanex.NexHR.Models.ScheduledMessage
{
    public class EditConversation
    {
        public int ConversationId { get; set; }

        public string SkypeConversationId { get; set; }

        public string ConversationName { get; set; }

        public int EditedBy { get; set; }

        public string PermittedEmployeeIds { get; set; }

        public bool IsActive { get; set; }
    }
}