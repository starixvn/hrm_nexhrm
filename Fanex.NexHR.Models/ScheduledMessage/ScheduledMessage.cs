namespace Fanex.NexHR.Models.ScheduledMessage
{
    using System;

    public class ScheduledMessage
    {
        public int ScheduledMessageId { get; set; }

        public string ConversationId { get; set; }

        public string ConversationName { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }

        public string NickName { get; set; }

        public bool IsSent { get; set; }
    }
}