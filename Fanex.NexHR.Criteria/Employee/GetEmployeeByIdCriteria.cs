namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "Employee_GetById";
    }
}
