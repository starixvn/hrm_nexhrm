namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetAllReminderLateInEarlyOut : CriteriaBase
    {
        public override string GetSettingKey() => "EmployeeAttendance_GetAllReminderLateInEarlyOut";
        public override bool IsValid() => true;
    }
}
