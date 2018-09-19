namespace Fanex.NexHR.Criteria.Workload
{
    using Data.Repository;

    public class GetEmployeeAttendanceLateInEarlyOutCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return EmpId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeAttendance_ReminderLateInEarlyOut";
        }
    }
}
