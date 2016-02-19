namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetLeaveHalfDayCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetLeaveHalfDay";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
