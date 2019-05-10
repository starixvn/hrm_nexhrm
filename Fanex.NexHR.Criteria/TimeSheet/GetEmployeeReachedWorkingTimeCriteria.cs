namespace Fanex.NexHR.Criteria.TimeSheet
{
    using Fanex.Data.Repository;

    public class GetEmployeeReachedWorkingTimeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeAttendance_GetEmployeeReachedWorkingTime";
        }
    }
}
