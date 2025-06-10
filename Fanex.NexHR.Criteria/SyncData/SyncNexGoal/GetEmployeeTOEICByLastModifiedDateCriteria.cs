using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetEmployeeTOEICByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "Sync_EmployeeTOEIC_Get";

    public override bool IsValid() => true;
}