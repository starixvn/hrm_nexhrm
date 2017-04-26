namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetLeaderAndHRCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetLeaderAndHR";
        }
    }
}
