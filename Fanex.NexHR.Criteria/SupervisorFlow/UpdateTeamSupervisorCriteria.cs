namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    using Data.Repository;

    public class UpdateTeamSupervisorCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string ApproverIds { get; set; }
        
        public int ModifiedBy { get; set; }

         public override string GetSettingKey() => "GroupSupervisor_Update";

        public override bool IsValid() => true;
    }
}
