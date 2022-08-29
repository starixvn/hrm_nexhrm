namespace Fanex.NexHR.Criteria.Employee
{
    using System;

    public class DeleteEmployeeCriteria : BaseCommand
    {
        public int EmployeeId { get; set; }

        public DateTime ResignedDate { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Employee_Delete";
        }
    }
}