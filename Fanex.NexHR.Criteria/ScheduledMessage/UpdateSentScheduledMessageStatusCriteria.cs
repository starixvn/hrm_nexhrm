namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class UpdateSentScheduledMessageStatusCriteria : CriteriaBase
    {
        public string ScheduledMessageIds { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_UpdateStatus";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ScheduledMessageIds);
    }
}