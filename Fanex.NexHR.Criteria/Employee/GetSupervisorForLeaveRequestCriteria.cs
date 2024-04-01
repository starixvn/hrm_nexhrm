namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetSupervisorForLeaveRequestCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public bool IsManagementApprove { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "Employee_GetSupervisorForLeave";
    }
}
