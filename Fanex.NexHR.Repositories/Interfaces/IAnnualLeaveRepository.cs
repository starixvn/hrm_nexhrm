namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.AnnualLeave;

    public interface IAnnualLeaveRepository
    {
        IList<LeaveSummary> GetAnnualLeaveBalance(ICriteria criteria);

        IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(ICriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(ICriteria criteria);

        IList<LeaveDeductionDetail> GetLeaveDeductionDetail(ICriteria criteria);
    }
}