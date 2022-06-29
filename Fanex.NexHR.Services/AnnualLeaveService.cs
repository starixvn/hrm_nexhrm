namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.AnnualLeave;
    using Interfaces;
    using Models.AnnualLeave;
    using Repositories.Interfaces;

    public class AnnualLeaveService : BaseService, IAnnualLeaveService
    {
        private readonly IAnnualLeaveRepository annualLeaveRepository;

        public AnnualLeaveService(IAnnualLeaveRepository annualLeaveRepository)
        {
            this.annualLeaveRepository = annualLeaveRepository;
        }

        public IList<LeaveSummary> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria)
        {
            IList<LeaveSummary> leaveSummary = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveSummary = annualLeaveRepository.GetAnnualLeaveBalance(criteria);
            }

            return leaveSummary ?? new List<LeaveSummary>();
        }

        public ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria)
        {
            ShowOpenAndCloseAnnualBalance showOpenAndCloseAnnualBalance = null;
            if (criteria != null && criteria.IsValid())
            {
                showOpenAndCloseAnnualBalance = GetDataEntity<ShowOpenAndCloseAnnualBalance>(criteria);
            }

            return showOpenAndCloseAnnualBalance;
        }

        public IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            IList<AnnualLeaveTransaction> annualLeaves = null;
            if (criteria != null && criteria.IsValid())
            {
                annualLeaves = annualLeaveRepository.GetAnnualLeaveTransactions(criteria);
            }

            return annualLeaves ?? new List<AnnualLeaveTransaction>();
        }

        public AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            AnnualLeaveTransaction annualLeave = null;
            if (criteria != null && criteria.IsValid())
            {
                annualLeave = annualLeaveRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return annualLeave;
        }

        public IList<LeaveConversionDetail> GetLeaveConversionDetail(GetLeaveConversionDetailCriteria criteria)
        {
            IList<LeaveConversionDetail> leaveDeductions = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveDeductions = annualLeaveRepository.GetLeaveConversionDetail(criteria);
            }

            return leaveDeductions ?? new List<LeaveConversionDetail>();
        }
    }
}