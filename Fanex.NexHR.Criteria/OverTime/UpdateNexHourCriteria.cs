namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Data.Repository;

    public class UpdateNexHourCriteria : CriteriaBase
    {
        public int ModifiedBy { get; set; }

        public int OvertimeId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public float AdjustHour { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_UpdateNexHour";
        }

        public override bool IsValid()
        {
            return ModifiedBy > 0 && OvertimeId > 0 && EmployeeId > 0 && AdjustHour >= 0;
        }
    }
}
