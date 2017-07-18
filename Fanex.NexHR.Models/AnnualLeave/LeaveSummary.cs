namespace Fanex.NexHR.Models.AnnualLeave
{
    public class LeaveSummary : TotalOfEachMonth
    {
        public Employee Employee { get; set; }

        public float AnnualLeaveLastYear { get; set; }

        public float AnnualLeaveCurrentYear { get; set; }

        public float LeaveBalance { get; set; }
    }
}