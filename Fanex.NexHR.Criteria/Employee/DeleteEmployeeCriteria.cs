namespace Fanex.NexHR.Criteria.Employee
{
    public class DeleteEmployeeCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => EmpId > 0;

        public override string GetSettingKey() => "Employee_Delete";
    }
}