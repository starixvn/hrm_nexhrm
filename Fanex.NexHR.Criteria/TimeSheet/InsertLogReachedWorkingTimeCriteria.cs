namespace Fanex.NexHR.Criteria.TimeSheet
{
    using System;
    using Fanex.Data.Repository;

    public class InsertLogReachedWorkingTimeCriteria : CriteriaBase
    {
        public string EmployeeIds { get; set; }

        public DateTime WorkDate { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmployeeIds);
        }

        public override string GetSettingKey()
        {
            return "LogReachedWorkingTime_Insert";
        }
    }
}
