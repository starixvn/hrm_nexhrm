namespace Fanex.NexHR.Criteria.Team
{
    using Data.Repository;

    public class GetTeamByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_GetById";
        }
    }
}
