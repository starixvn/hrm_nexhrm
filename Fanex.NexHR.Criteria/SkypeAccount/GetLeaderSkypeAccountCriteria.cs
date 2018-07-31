namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class GetLeaderSkypeAccountCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "SkypeAccount_GetLeaders";

        public override bool IsValid() => EmployeeId > 0;
    }
}