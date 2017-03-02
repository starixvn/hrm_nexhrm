namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class InsertEmployeeCalendarCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool SetOffDay { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_Insert";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}