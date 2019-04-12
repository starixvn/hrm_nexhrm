namespace Fanex.NexHR.Models.ScheduledMessage
{
    using System.Collections.Generic;

    public class Conversation
    {
        public int Id { get; set; }

        public string ConversationId { get; set; }

        public string ConversationName { get; set; }

        public IEnumerable<PermittedEmployee> PermittedEmployees { get; set; }
    }
}