namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Report_GetLateInEarlyOut";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
