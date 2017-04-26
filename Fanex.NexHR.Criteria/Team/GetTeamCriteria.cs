namespace Fanex.NexHR.Criteria.Team
{
    using Data.Repository;

    public class GetTeamCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_GetGroups";
        }
    }
}
