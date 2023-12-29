namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.AnnualLeave;
    using System.Collections.Generic;

    public interface IAnnualLeaveRepository
    {
        Task<IList<LeaveSummary>> GetAnnualLeaveBalance(ICriteria criteria);

        Task<IList<AnnualLeaveTransaction>> GetAnnualLeaveTransactions(ICriteria criteria);

        Task<AnnualLeaveTransaction> GetAnnualLeaveTransactionById(ICriteria criteria);

        Task<IList<LeaveConversionDetail>> GetLeaveConversionDetail(ICriteria criteria);
    }
}