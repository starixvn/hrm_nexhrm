using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetWorkingTimeRequestsByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "Sync_WorkingTimeRequest_Get";

    public override bool IsValid() => true;
}