namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    public class UpdateSentScheduledMessageStatusCriteria : BaseCommand
    {
        public string ScheduledMessageIds { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_UpdateStatus";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ScheduledMessageIds);
    }
}