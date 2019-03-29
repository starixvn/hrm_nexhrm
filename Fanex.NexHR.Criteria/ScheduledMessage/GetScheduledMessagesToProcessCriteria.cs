namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;
    using System;

    public class GetScheduledMessagesToProcessCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "ScheduledMessage_GetMessagesToProcess";

        public override bool IsValid() => true;
    }
}