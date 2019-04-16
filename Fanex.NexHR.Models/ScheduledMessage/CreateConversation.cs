namespace Fanex.NexHR.Models.ScheduledMessage
{
    public class CreateConversation
    {
        public string SkypeConversationId { get; set; }

        public string ConversationName { get; set; }

        public int CreatedBy { get; set; }

        public string PermittedEmployeeIds { get; set; }
    }
}