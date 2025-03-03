namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetEmployeeMissedLogTimeCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey()
        {
            return "Employee_GetMissedLogTime";
        }
    }
}
