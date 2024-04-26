namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    public class UpdateTeamSupervisorCriteria : BaseCommand
    {
        public int GroupId { get; set; }

        public string ApproverIds { get; set; }
        
        public int ModifiedBy { get; set; }

         public override string GetSettingKey() => "GroupSupervisor_Update";

         public override bool IsValid()
        {
            return GroupId > 0 && ModifiedBy > 0;
        }
    }
}
