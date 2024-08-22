namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Data;

    public class InsertEmployeeCalendarCommand : BaseCommand
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool SetOffDay { get; set; }

        public override string GetSettingKey() => "EmployeeCalendar_Insert";

        public override bool IsValid() => FromDate <= ToDate;
    }
}