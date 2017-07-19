namespace Fanex.NexHR.Models
{
    public class WorkingHoursStatistic
    {
        public Employee Employee { get; set; }

        public int TotalWorkingDay { get; set; }

        public float ActualWorkingDay { get; set; }

        public float AnnualLeave { get; set; }

        public float PaidLeave { get; set; }

        public float SickLeave { get; set; }

        public float InsuranceLeave { get; set; }

        public float UnpaidLeave { get; set; }

        public float PreMonthSickLeave { get; set; }

        public float PreMonthUnpaidLeave { get; set; }
    }
}
