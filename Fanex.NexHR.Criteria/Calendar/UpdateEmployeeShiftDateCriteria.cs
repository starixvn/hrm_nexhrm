namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateEmployeeShiftDateCriteria : CriteriaBase
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
