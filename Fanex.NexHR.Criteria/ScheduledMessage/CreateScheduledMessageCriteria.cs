namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using System;
    using Fanex.Data.Repository;

    public class CreateScheduledMessageCriteria : CriteriaBase
    {
        public int ConversationId { get; set; }

        public DateTime SendDate { get; set; }

        public string Content { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Insert";

        public override bool IsValid() => ConversationId > 0 && !string.IsNullOrEmpty(Content) && CreatedBy > 0;
    }
}