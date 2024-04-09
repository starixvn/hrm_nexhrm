namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetEmployeeByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username);
        }

        public override string GetSettingKey()
        {
            return "Employee_GetByUsername";
        }
    }
}
