namespace Fanex.NexHR.Criteria.Team
{
    using Data.Repository;

    public class GetTeamByManagerIdCriteria : CriteriaBase
    {
        public int ManagerId { get; set; }

        public override bool IsValid()
        {
            return ManagerId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_GetGroupsByManagerID";
        }
    }
}
