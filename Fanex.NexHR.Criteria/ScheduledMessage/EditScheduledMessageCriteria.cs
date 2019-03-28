namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;
    using System;

    public class EditScheduledMessageCriteria : CriteriaBase
    {
        public int ScheduledMessageId { get; set; }

        public string ConversationId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Update";

        public override bool IsValid() => !string.IsNullOrEmpty(ConversationId) && !string.IsNullOrEmpty(Content) && ScheduledMessageId > 0;
    }
}