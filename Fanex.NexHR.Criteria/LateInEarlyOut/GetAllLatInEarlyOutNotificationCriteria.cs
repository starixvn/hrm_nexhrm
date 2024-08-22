namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Data.Repository;
    using System;

    public class GetAllLatInEarlyOutNotificationCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "LateInEarlyOut_GetAllEmployee";
    }
}
