using System;
using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetEmployeeCalendarsByLastModifiedDateCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Sync_EmployeeCalendar_Get";

    public override bool IsValid() => true;
}