namespace Fanex.NexHR.Models.AnnualLeave
{
    using System.Collections.Generic;

    public class LeaveSummaryTransactions
    {
        public IList<LeaveSummary> leaveSummaries { get; set; }
        public IList<AnnualLeaveTransaction> annualLeaveTransactions { get; set; }
    }
}
