namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    public class GetLeaveScheduleCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmployeeId { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int? TeamId { get; set; }

        public override bool IsValid() => FromDate <= ToDate;

        public override string GetSettingKey() => "EmployeeCalendar_GetLeaveSchedule";
    }
}