namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class OvertimeSummaryCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int? TeamId { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetSummary";
        }

        public override bool IsValid()
        {
            return Year >= 2010;
        }
    }
}