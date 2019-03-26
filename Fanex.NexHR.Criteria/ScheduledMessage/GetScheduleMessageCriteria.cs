namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetScheduleMessageCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Get";

        public override bool IsValid() => Year > 0;
    }
}