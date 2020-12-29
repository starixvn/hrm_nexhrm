namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class SyncDataGetDepartmentCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public bool IsNexHRM { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey() => IsNexHRM ? "SyncData_NexHRM_GetDepartmentByLastModifiedDate" : "SyncData_LumiHRM_GetDepartmentByLastModifiedDate";

        public override bool IsValid() => true;
    }
}