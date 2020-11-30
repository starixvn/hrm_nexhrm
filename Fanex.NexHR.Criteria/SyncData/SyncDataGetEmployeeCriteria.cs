namespace Fanex.NexHR.Criteria.SyncData
{
    using System;
    using Fanex.Data.Repository;

    public class SyncDataGetEmployeeCriteria : CriteriaBase
    {
        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey() => "SyncData_GetEmployeeByLastModifiedDate";

        public override bool IsValid() => true;
    }
}