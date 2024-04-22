namespace Fanex.NexHR.Criteria.Employee
{
    public class GetApproverByIdCriteria : BaseCommand
    {

        public int EmpId { get; set; }

        public override string GetSettingKey() => "EmployeeSupervisor_GetById";

        public override bool IsValid() => EmpId > 0;
    }
}
