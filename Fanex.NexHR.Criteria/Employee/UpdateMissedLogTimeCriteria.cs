namespace Fanex.NexHR.Criteria.Employee
{
    public class UpdateMissedLogTimeCriteria : BaseCommand
    {

        public string EmpIDs { get; set; }

        public override string GetSettingKey() => "EmployeeLogTimeMissed_UpdateMultiple";

        public override bool IsValid() => !string.IsNullOrEmpty(EmpIDs);
    }
}
