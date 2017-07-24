
namespace Fanex.NexHR.Criteria.Report
{
    using Data;
    using Data.Repository;
    using System;

    /// <summary>
    /// Get total working hours summary criteria.
    /// </summary>
    public class GetWorkingHoursStatisticCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "Report_WorkingHoursStatistic";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
