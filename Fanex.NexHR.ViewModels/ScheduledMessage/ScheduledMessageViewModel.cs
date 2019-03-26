namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using System;

    public class ScheduledMessageViewModel
    {
        public int ScheduledMessageId { get; set; }

        public string ConversationId { get; set; }

        public string GroupName { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }

        public string NickName { get; set; }
    }
}