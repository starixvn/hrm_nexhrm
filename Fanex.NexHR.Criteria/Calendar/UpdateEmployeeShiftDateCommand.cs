namespace Fanex.NexHR.Criteria.Calendar
{
    using System;

    public class UpdateEmployeeShiftDateCommand : BaseCommand
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public int? ShiftId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "EmployeeCalendar_Update";
    }
}