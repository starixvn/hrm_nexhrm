namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;
    using System;

    public class GetReportOvertimeHRCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public int? CreatedBy { get; set; }

        public int Verified { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_ReportHR";
        }

        public override bool IsValid()
        {
            return DateTime.Compare(FromDate, ToDate) <= 0;
        }
    }
}
