namespace Fanex.NexHR.Criteria.SyncData
{
    using System;
    using Fanex.Data.Repository;

    public class GetUnitHeadScheduleCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Sync_GO_UnitHead_GetSchedule";

        public override bool IsValid() => true;
    }

}