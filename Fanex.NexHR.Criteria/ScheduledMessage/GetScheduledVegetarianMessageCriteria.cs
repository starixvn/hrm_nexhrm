namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetScheduledVegetarianMessageCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "ScheduledVegetarianMessage_Get";

        public override bool IsValid() => true;
    }
}
