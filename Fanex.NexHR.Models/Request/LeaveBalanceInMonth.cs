namespace Fanex.NexHR.Models.Request
{
    public class LeaveBalanceInMonth
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        public decimal AnnualLeaveCanBeUsed { get; set; }
    }
}
