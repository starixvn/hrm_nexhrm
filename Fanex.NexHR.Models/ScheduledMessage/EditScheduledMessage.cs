namespace Fanex.NexHR.Models.ScheduledMessage
{
    using System;

    public class EditScheduledMessage
    {
        public int ScheduledMessageId { get; set; }

        public int ConversationId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }
        public string CronExpression { get; set; }
        public int StepNumber { get; set; }
        public int? RepeatTypeId { get; set; }
    }
}