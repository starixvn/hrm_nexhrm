namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    using Data.Repository;

    public class GetTeamSupervisorCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

         public override string GetSettingKey() => "GroupSupervisor_Get";

        public override bool IsValid() => true;
    }
}
