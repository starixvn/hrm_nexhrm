namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetScheduleMessagesCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_Get";

        public override bool IsValid() => Year > 0;
    }
}