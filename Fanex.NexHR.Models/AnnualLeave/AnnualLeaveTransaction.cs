namespace Fanex.NexHR.Models.AnnualLeave
{
    using System;

    public class AnnualLeaveTransaction
    {
        public int LeaveTransactionId { get; set; }

        public Employee Employee { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public DateTime AffectedFrom { get; set; }

        public long? RequestId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
