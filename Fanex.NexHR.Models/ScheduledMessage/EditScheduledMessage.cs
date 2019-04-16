namespace Fanex.NexHR.Models.ScheduledMessage
{
    using System;

    public class EditScheduledMessage
    {
        public int ScheduledMessageId { get; set; }

        public int ConversationId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }
    }
}