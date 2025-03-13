namespace Fanex.NexHR.Criteria.ManagementGrade
{
    using Data.Repository;

    public class GetEmployeeManagementGradeCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }
        public bool IsOnlyCurrentUsed { get; set; } = true;

        public override string GetSettingKey() => "EmployeeManagementGrade_Get";

        public override bool IsValid() => true;
    }
}
