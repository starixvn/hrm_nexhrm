namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    public class UpdatePersonalSupervisorCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public string ApproverIds { get; set; }
        
        public int ModifiedBy { get; set; }

         public override string GetSettingKey() => "EmployeeSupervisor_Update";

         public override bool IsValid()
        {
            return EmpId > 0 && ModifiedBy > 0;
        }
    }
}
