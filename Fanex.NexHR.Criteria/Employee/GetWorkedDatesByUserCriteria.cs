namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Data.Repository;

    public class GetWorkedDatesByUserCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "EmployeeCalender_GetCurrentUserWorkedDates";

        public override bool IsValid() => UserId > 0 && FromDate < ToDate;
    }
}