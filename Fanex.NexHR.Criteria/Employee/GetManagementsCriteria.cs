namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetManagementsCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetManagements";
        }
    }
}