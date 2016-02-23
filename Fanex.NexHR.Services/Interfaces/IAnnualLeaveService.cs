namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeave;

    public interface IAnnualLeaveService
    {
        IList<LeaveSummary> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria);

        ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria);

        IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria);

        CommandResult DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        CommandResult UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        CommandResult InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

        CommandResult AdjustAnnualLeaves(AdjustAnnualLeavesCriteria criteria);

        CommandResult OpenAnnualLeaves(OpenAnnualBalanceForNewYearCriteria criteria);

        CommandResult ResetAnnualLeaves(ResetAnnualLeavesCriteria criteria);

        CommandResult AddLeaveDeduction(InsertLeaveDeductionCriteria criteria);

        CommandResult EditLeaveDeduction(UpdateLeaveDeductionCriteria criteria);

        CommandResult DeleteLeaveDeduction(DeleteLeaveDeductionCriteria criteria);
    }
}