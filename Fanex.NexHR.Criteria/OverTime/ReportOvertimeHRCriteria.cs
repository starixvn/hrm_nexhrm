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

        public override string GetSettingKey()
        {
            return "Overtime_ReportHR";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
