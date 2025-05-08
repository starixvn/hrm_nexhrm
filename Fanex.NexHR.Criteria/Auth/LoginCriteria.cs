namespace Fanex.NexHR.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class LoginCriteria : CriteriaBase
    {
        public string DomainUsername { get; set; }

        public string Password { get; set; }

        public override string GetSettingKey() => "EmployeeExt_Login";

        public override bool IsValid() => !string.IsNullOrEmpty(DomainUsername) && !string.IsNullOrEmpty(Password);
    }
}
