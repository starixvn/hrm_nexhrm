namespace Fanex.NexHR.Models.AnnualLeave
{
    using System;

    public class LeaveConversionDetail
    {
        public int LeaveConversionId { get; set; }

        public Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public bool IsAnnual { get; set; }

        public float LeaveDays { get; set; }

        public string Description { get; set; }

        public Employee ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
