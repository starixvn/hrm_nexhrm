namespace Fanex.NexHR.Criteria.SyncData
{
    using System;
    using Fanex.Data.Repository;

    public class SyncDataGetDepartmentCriteria : CriteriaBase
    {
        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey() => "SyncData_GetDepartmentByLastModifiedDate";

        public override bool IsValid() => true;
    }
}