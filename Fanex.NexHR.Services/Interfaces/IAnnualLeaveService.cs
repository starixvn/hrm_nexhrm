namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models.AnnualLeave;

    public interface IAnnualLeaveService
    {
        IList<LeaveSummary> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria);

        ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria);

        IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria);

        IList<LeaveDeductionDetail> GetLeaveDeductionDetail(GetLeaveDeductionDetailCriteria criteria);
    }
}