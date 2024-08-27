namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Data.Repository;

    public class ReportOvertimeHRCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? CreatedBy { get; set; }

        public int Status { get; set; }

        public override string GetSettingKey() => "Overtime_ReportHR";

        public override bool IsValid() => FromDate <= ToDate;
    }
}
