namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.AnnualLeave;
    using Models.AnnualLeave;

    public interface IAnnualLeaveService
    {
        Task<IList<LeaveSummary>> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria);

        Task<IList<AnnualLeaveTransaction>> GetAnnualLeaveTransactionsByYear(GetAnnualLeaveTransactionByYearCriteria criteria);

        ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria);

        Task<IList<AnnualLeaveTransaction>> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria);

        IList<LeaveConversionDetail> GetLeaveConversionDetail(GetLeaveConversionDetailCriteria criteria);
    }
}