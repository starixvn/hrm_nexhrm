using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetJobTitleByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "Sync_JobTitle_Get";

    public override bool IsValid() => true;
}