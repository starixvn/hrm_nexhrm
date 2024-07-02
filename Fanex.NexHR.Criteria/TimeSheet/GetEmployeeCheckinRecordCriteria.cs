namespace Fanex.NexHR.Criteria.TimeSheet
{
    using Fanex.Data.Repository;

    public class GetEmployeeCheckinRecordCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EmployeeAttendance_GetCheckInTimeToNotify";

        public override bool IsValid() => true;
    }
}