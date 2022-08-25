namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.AnnualLeave;

    public interface IAnnualLeaveRepository
    {
        Task<IList<LeaveSummary>> GetAnnualLeaveBalance(ICriteria criteria);

        Task<IList<AnnualLeaveTransaction>> GetAnnualLeaveTransactions(ICriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(ICriteria criteria);

        IList<LeaveConversionDetail> GetLeaveConversionDetail(ICriteria criteria);
    }
}