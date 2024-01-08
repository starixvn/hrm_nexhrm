namespace Fanex.NexHR.Criteria.JobGrade
{
    using Data.Repository;

    public class GetEmployeeJobGradeCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }
        public bool IsOnlyCurrentUsed { get; set; } = true;

        public override string GetSettingKey() => "EmployeeJobGrade_Get";

        public override bool IsValid() => true;
    }
}
