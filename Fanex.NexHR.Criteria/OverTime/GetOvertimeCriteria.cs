namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Data.Repository;

    public class GetOvertimeCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? CreatedBy { get; set; }

        public int Status { get; set; }

        public override string GetSettingKey() => "Overtime_ReportSummary";

        public override bool IsValid() => FromDate <= ToDate;
    }
}
