namespace Fanex.NexHR.Models.ScheduledMessage
{
    using System.Collections.Generic;

    public class Conversation
    {
        public int ConversationId { get; set; }

        public string SkypeConversationId { get; set; }

        public string ConversationName { get; set; }

        public IEnumerable<PermittedEmployee> PermittedEmployees { get; set; }

        public bool IsActive { get; set; }
    }
}