namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using System;

    public class ReportDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int CreatedBy { get; set; }

        public int EmployeeId { get; set; }

        public int Status { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_ReportDetail";
        }
    }
}
