namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    public class ConversationViewModel
    {
        public int ConversationId { get; set; }

        public string SkypeConversationId { get; set; }

        public string ConversationName { get; set; }

        public bool IsActive { get; set; }

        public string PermittedEmployees { get; set; }
    }
}