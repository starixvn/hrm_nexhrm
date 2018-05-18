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

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetLeaveSchedule";
        }
    }
}