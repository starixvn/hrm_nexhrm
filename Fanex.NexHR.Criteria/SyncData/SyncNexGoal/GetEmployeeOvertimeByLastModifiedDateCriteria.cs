using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetEmployeeOvertimeByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "Sync_EmployeeOverTime_Get";

    public override bool IsValid() => true;
}