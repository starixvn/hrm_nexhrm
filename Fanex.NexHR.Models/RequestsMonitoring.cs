namespace Fanex.NexHR.Models
{
    public class RequestsMonitoring
    {
        public Employee Requester { get; set; }

        public int TotalLeaveTimes { get; set; }

        public float TotalAnnualLeave { get; set; }

        public float TotalUnpaidLeave { get; set; }

        public float TotalOtherLeave { get; set; }

        public int TotalCheckInOutTimes { get; set; }

        public int TotalMissingCheckInOutTimes { get; set; }

        public int LateInEarlyOutTimes { get; set; }

        public int TotalAbsentTimes { get; set; }
    }
}
