namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class SyncDataGetEmployeeCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public bool IsNexHRM { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey() => IsNexHRM ? "SyncData_NexHRM_GetEmployeeByLastModifiedDate" : "SyncData_LumiHRM_GetEmployeeByLastModifiedDate";

        public override bool IsValid() => true;
    }
}