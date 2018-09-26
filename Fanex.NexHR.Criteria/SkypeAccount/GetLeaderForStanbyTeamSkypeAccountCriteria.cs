namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class GetLeaderForStanbyTeamSkypeAccountCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SkypeAccount_GetLeaderForStanbyTeam";

        public override bool IsValid() => true;
    }
}