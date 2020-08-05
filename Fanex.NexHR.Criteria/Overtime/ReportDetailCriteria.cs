namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Data.Repository;

    public class GetEmployeeOvertimeCriteria : CriteriaBase
    {
        public int? OvertimeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? EmployeeId { get; set; }

        public int Status { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_ReportDetail";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
