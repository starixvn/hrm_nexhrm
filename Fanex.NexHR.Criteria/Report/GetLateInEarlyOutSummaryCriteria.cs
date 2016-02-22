namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutSummaryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Report_LateInEarlyOutSummary";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}