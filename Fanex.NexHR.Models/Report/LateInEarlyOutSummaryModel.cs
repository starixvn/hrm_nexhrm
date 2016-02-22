namespace Fanex.NexHR.Models.Report
{
    using System.Collections.Generic;

    public class LateInEarlyOutSummaryModel
    {
        public LateInEarlyOutSummaryModel()
        {
            LateInEarlyOutSummaryDetails = new List<LateInEarlyOutSummaryDetailModel>();
        }

        public Employee Employee { get; set; }

        public int TotalTimes { get; set; }

        public IList<LateInEarlyOutSummaryDetailModel> LateInEarlyOutSummaryDetails { get; set; }
    }
}