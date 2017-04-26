namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.AnnualLeave;
    using Models.AnnualLeave;

    public interface IAnnualLeaveService
    {
        IList<LeaveSummary> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria);

        ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria);

        IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria);

        IList<LeaveConversionDetail> GetLeaveConversionDetail(GetLeaveConversionDetailCriteria criteria);
    }
}