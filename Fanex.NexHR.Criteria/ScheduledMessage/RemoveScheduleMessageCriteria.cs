namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    public class RemoveScheduleMessageCriteria : BaseCommand
    {
        public int ScheduledMessageId { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Delete";

        public override bool IsValid() => ScheduledMessageId > 0;
    }
}