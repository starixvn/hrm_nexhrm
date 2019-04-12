namespace Fanex.NexHR.Models.ScheduledMessage
{
    public class EditConversation
    {
        public int Id { get; set; }

        public string ConversationId { get; set; }

        public string ConversationName { get; set; }

        public int EditedBy { get; set; }

        public string PermittedEmployeeIds { get; set; }
    }
}