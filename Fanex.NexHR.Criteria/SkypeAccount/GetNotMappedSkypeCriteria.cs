namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class GetNotMappedSkypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Employee_GetNotMappedSkypeAccount";

        public override bool IsValid() => true;
    }
}