namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    public class InsertPersonalSupervisorCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public string ApproverIds { get; set; }
        
        public int ModifiedBy { get; set; }

         public override string GetSettingKey() => "EmployeeSupervisor_Insert";

         public override bool IsValid()
        {
            return EmpId > 0 && ModifiedBy > 0;
        }
    }
}
