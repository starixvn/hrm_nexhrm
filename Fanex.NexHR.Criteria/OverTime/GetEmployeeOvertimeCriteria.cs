namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Data.Repository;

    public class GetMyOvertimeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Overtime_MyReport";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}
