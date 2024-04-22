namespace Fanex.NexHR.Criteria.Employee
{
    public class UpdateApproverCriteria : BaseCommand
    {

        public int EmpId { get; set; }

        public string ApproverIds { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "EmployeeSupervisor_Update";

        public override bool IsValid() => ModifiedBy > 0 && EmpId > 0;
    }
}
