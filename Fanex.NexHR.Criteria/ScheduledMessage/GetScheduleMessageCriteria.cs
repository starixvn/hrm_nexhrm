namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetScheduleMessageCriteria : CriteriaBase
    {
        public int ScheduledMessageId { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_GetById";

        public override bool IsValid() => ScheduledMessageId > 0;
    }
}