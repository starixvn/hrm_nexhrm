namespace Fanex.NexHR.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class GetLoginEmployeeCriteria : CriteriaBase
    {
        public string DomainUsername { get; set; }

        public override string GetSettingKey() => "EmployeeExt_GetLoginEmployee";

        public override bool IsValid() => !string.IsNullOrEmpty(DomainUsername);
    }
}
