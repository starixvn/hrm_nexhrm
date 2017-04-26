namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.AnnualLeave;

    public interface IAnnualLeaveRepository
    {
        IList<LeaveSummary> GetAnnualLeaveBalance(ICriteria criteria);

        IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(ICriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(ICriteria criteria);

        IList<LeaveConversionDetail> GetLeaveConversionDetail(ICriteria criteria);
    }
}