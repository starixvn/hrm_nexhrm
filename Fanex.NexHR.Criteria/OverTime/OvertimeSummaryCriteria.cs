namespace Fanex.NexHR.Criteria.Overtime
{
    using Data;
    using Data.Repository;

    public class OvertimeSummaryCriteria : CriteriaBase
    {
        private const int year = 2010;

        public int? EmployeeId { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int? TeamId { get; set; }

        public int? DepartmentId { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey() => "Overtime_GetSummary";

        public override bool IsValid() => Year >= year;
    }
}