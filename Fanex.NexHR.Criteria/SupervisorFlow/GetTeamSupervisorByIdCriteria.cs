namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    using Fanex.Data.Repository;

    public class GetTeamSupervisorByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override string GetSettingKey() => "GroupSupervisor_GetById";

        public override bool IsValid() => GroupId > 0;
    }
}
