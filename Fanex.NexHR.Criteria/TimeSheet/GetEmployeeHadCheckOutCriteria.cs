namespace Fanex.NexHR.Criteria.TimeSheet
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeHadCheckOutCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EmployeeAttendance_GetCheckOutTimeToNotify";

        public override bool IsValid() => true;
    }
}