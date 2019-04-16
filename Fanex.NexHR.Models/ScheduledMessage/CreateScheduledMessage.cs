namespace Fanex.NexHR.Models.ScheduledMessage
{
    using System;

    public class CreateScheduledMessage
    {
        public int ConversationId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }
    }
}