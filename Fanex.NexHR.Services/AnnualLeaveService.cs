namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.AnnualLeave;
    using Interfaces;
    using Models.AnnualLeave;

    public class AnnualLeaveService : IAnnualLeaveService
    {
        private readonly IBaseService baseService;

        public AnnualLeaveService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria)
        {
            var leaveSummary = await baseService.GetDataAsync<LeaveSummary>(criteria);
            return leaveSummary;
        }

        public async Task<ShowOpenAndCloseAnnualBalance> GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria)
        {
            var showOpenAndCloseAnnualBalance = await baseService.GetDataEntityAsync<ShowOpenAndCloseAnnualBalance>(criteria);
            return showOpenAndCloseAnnualBalance;
        }

        public async Task<IEnumerable<AnnualLeaveTransaction>> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            var leaveTransactions = await baseService.GetDataAsync<AnnualLeaveTransaction>(criteria);
            return leaveTransactions;
        }

        public async Task<AnnualLeaveTransaction> GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            var leaveTransaction = await baseService.GetDataEntityAsync<AnnualLeaveTransaction>(criteria);
            return leaveTransaction;
        }

        public async Task<IEnumerable<LeaveConversionDetail>> GetLeaveConversionDetail(GetLeaveConversionDetailCriteria criteria)
        {
            var leaveDeductions = await baseService.GetDataAsync<LeaveConversionDetail>(criteria);
            return leaveDeductions;
        }
    }
}