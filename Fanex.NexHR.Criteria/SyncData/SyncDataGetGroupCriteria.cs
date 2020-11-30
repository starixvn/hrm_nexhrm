namespace Fanex.NexHR.Criteria.SyncData
{
    using System;
    using Fanex.Data.Repository;

    public class SyncDataGetGroupCriteria : CriteriaBase
    {
        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey() => "SyncData_GetGroupByLastModifiedDate";

        public override bool IsValid() => true;
    }
}