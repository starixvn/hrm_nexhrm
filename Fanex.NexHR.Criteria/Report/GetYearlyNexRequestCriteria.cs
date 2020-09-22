namespace Fanex.NexHR.Criteria.Report
{
    using Data;
    using Data.Repository;

    public class GetYearlyNexRequestCriteria : CriteriaBase
    {
        private const int minYear = 2010;

        public int? EmployeeId { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int? TeamId { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_YearlyNexRequest";
        }

        public override bool IsValid()
        {
            return Year >= minYear;
        }
    }
}