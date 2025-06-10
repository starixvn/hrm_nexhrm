using Fanex.Data.Repository;

namespace Fanex.NexHR.Criteria.SyncData.SyncNexGoal;

public class GetEmployeeAttendancesCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Sync_EmployeeAttendance_Get";

    public override bool IsValid() => true;
}