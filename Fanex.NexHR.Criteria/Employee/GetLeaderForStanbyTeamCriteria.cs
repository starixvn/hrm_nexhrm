namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetLeaderForStanbyTeamCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetLeaderForStanbyTeam";
        }
    }
}
