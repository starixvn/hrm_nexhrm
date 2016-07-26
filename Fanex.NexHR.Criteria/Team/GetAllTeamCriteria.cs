namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetAllTeamCriteria : CriteriaBase
    {
        public bool IncludeSpecialTeam { get; set; }

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
