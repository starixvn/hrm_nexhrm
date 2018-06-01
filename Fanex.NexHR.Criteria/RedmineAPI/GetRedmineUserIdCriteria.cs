namespace Fanex.NexHR.Criteria.RedmineAPI
{
    using Fanex.Data.Repository;

    public class GetRedmineUserIdCriteria : CriteriaBase
    {
        public string LoginName { get; set; }

        public override string GetSettingKey() => "Employee_GetRedmineUserByDomainName";

        public override bool IsValid() => !string.IsNullOrEmpty(LoginName);
    }
}