namespace Fanex.NexHR.Criteria.Calendar
{
    using System;

    public class UpdateEmployeeShiftDateCriteria : BaseCommand
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public int? ShiftId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_Update";
        }
    }
}