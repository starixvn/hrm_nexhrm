namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using System;

    public class GetMyReportCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_MyReport";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FromDate <= ToDate;
        }
    }
}
