namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.AnnualLeave;
    using Interfaces;
    using Models.AnnualLeave;
    using Repositories.Interfaces;

    public class AnnualLeaveService : IAnnualLeaveService
    {
        private readonly IAnnualLeaveRepository annualLeaveRepository;
        private readonly IBaseService baseService;

        public AnnualLeaveService(IAnnualLeaveRepository annualLeaveRepository, IBaseService baseService)
        {
            this.annualLeaveRepository = annualLeaveRepository;
            this.baseService = baseService;
        }

        public async Task<IList<LeaveSummary>> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria)
        {
            IList<LeaveSummary> leaveSummary = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveSummary = await annualLeaveRepository.GetAnnualLeaveBalance(criteria);
            }

            return leaveSummary ?? new List<LeaveSummary>();
        }

        public async Task<ShowOpenAndCloseAnnualBalance> GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria)
        {
            ShowOpenAndCloseAnnualBalance showOpenAndCloseAnnualBalance = null;
            if (criteria != null && criteria.IsValid())
            {
                showOpenAndCloseAnnualBalance = await baseService.GetDataEntityAsync<ShowOpenAndCloseAnnualBalance>(criteria);
            }

            return showOpenAndCloseAnnualBalance;
        }

        public async Task<IList<AnnualLeaveTransaction>> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            IList<AnnualLeaveTransaction> annualLeaves = null;
            if (criteria != null && criteria.IsValid())
            {
                annualLeaves = await annualLeaveRepository.GetAnnualLeaveTransactions(criteria);
            }

            return annualLeaves ?? new List<AnnualLeaveTransaction>();
        }

        public async Task<AnnualLeaveTransaction> GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            AnnualLeaveTransaction annualLeave = null;
            if (criteria != null && criteria.IsValid())
            {
                annualLeave = await annualLeaveRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return annualLeave;
        }

        public async Task<IList<LeaveConversionDetail>> GetLeaveConversionDetail(GetLeaveConversionDetailCriteria criteria)
        {
            IList<LeaveConversionDetail> leaveDeductions = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveDeductions = await annualLeaveRepository.GetLeaveConversionDetail(criteria);
            }

            return leaveDeductions ?? new List<LeaveConversionDetail>();
        }
    }
}