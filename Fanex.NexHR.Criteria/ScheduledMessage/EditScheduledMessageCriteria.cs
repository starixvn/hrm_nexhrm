namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using System;

    public class EditScheduledMessageCriteria : BaseCommand
    {
        public int ScheduledMessageId { get; set; }

        public int ConversationId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }
        public string CronExpression { get; set; }
        public int StepNumber { get; set; }
        public int? RepeatTypeId { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Update";

        public override bool IsValid() => ConversationId > 0 && !string.IsNullOrEmpty(Content) && ScheduledMessageId > 0;
    }
}