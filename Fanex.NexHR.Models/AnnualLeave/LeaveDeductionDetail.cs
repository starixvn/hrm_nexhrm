namespace Fanex.NexHR.Models.AnnualLeave
{
    using System;

    public class LeaveDeductionDetail
    {
        public int LeaveDeductionId { get; set; }

        public Employee Employee { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public float LeaveDays { get; set; }

        public string Description { get; set; }

        public Employee ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
