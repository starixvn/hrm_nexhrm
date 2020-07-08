namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMissingCheckInCheckOutCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EmployeeAttendance_GetMissingCheckInCheckOut";

        public override bool IsValid() => true;
    }
}