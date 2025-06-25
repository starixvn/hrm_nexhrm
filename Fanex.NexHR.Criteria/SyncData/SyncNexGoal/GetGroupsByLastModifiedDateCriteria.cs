using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetGroupsByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }
    public override string GetSettingKey() => "Sync_Group_Get";
    public override bool IsValid() => true;
}