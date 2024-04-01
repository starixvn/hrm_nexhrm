namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetSupervisorForCheckInOutRequestCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public bool IsStandbyAtWeekend { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "Employee_GetSupervisorForCheckInOut";
    }
}
