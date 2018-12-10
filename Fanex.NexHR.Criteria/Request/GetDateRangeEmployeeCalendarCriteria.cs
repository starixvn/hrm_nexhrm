namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Data;
    using Data.Repository;

    public class GetDateRangeEmployeeCalendarCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public long? RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetRangeDate";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FromDate <= ToDate;
        }
    }
}
