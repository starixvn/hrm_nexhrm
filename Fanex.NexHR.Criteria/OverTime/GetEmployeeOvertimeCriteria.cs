namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Data.Repository;

    public class GetMyOvertimeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

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
