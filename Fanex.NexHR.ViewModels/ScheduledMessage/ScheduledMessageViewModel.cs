namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using System;

    public class ScheduledMessageViewModel
    {
        public int ScheduledMessageId { get; set; }

        public int ConversationId { get; set; }

        public string SkypeConversationId { get; set; }

        public string ConversationName { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }

        public string NickName { get; set; }

        public bool IsSent { get; set; }
    }
}