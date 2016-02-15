namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByDomainNameCriteria : CriteriaBase
    {
        public string DomainUserName { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DomainUserName);
        }

        public override string GetSettingKey()
        {
            return "Employee_GetByDomainName";
        }
    }
}
