namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class GetManagementSkypeAccountCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SkypeAccount_GetManagements";

        public override bool IsValid() => true;
    }
}