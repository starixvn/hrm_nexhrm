namespace Fanex.NexHR.Models.AnnualLeave
{
    using System;

    public class LeaveConversionDetail
    {
        public int LeaveConversionId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public DateTime Date { get; set; }

        public bool IsAnnual { get; set; }

        public float LeaveDays { get; set; }

        public string Description { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
