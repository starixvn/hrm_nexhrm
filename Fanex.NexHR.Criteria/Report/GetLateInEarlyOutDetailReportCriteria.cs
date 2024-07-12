namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Data.Repository;

    public class GetLateInEarlyOutDetailReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public bool IncludeLeaveDeduction { get; set; }

        public override string GetSettingKey()
        {
            return "Report_GetLateInEarlyOutDetail";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
