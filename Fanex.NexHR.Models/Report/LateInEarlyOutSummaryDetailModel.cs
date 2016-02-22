namespace Fanex.NexHR.Models.Report
{
    using System;

    public class LateInEarlyOutSummaryDetailModel
    {
        public int EmpId { get; set; }

        public DateTime Month { get; set; }

        public int TotalTimes { get; set; }
    }
}