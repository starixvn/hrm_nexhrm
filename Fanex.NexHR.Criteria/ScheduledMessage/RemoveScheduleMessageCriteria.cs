namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class RemoveScheduleMessageCriteria : CriteriaBase
    {
        public int ScheduledMessageId { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Delete";

        public override bool IsValid() => ScheduledMessageId > 0;
    }
}