namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Data.Repository;

    public class DeleteEmployeeCriteria : CriteriaBase
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
