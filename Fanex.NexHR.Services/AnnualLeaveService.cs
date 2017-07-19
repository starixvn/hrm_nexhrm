namespace Fanex.NexHR.Services
{
    using Criteria.AnnualLeave;
    using Interfaces;
    using Models.AnnualLeave;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;

    public class AnnualLeaveService : BaseService, IAnnualLeaveService
    {
        private readonly IAnnualLeaveRepository _annualLeaveRepository;

        public AnnualLeaveService(IAnnualLeaveRepository annualLeaveRepository = null)
        {
            _annualLeaveRepository = annualLeaveRepository ?? new AnnualLeaveRepository();
        }

        public IList<LeaveSummary> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria)
        {
            IList<LeaveSummary> leaveSummary = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveSummary = _annualLeaveRepository.GetAnnualLeaveBalance(criteria);
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
                annualLeaves = _annualLeaveRepository.GetAnnualLeaveTransactions(criteria);
            }

            return annualLeaves ?? new List<AnnualLeaveTransaction>();
        }

        public AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            AnnualLeaveTransaction annualLeave = null;
            if (criteria != null && criteria.IsValid())
            {
                annualLeave = _annualLeaveRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return annualLeave;
        }

        public IList<LeaveConversionDetail> GetLeaveConversionDetail(GetLeaveConversionDetailCriteria criteria)
        {
            IList<LeaveConversionDetail> leaveDeductions = null;
            if (criteria != null && criteria.IsValid())
            {
                leaveDeductions = _annualLeaveRepository.GetLeaveConversionDetail(criteria);
            }

            return leaveDeductions ?? new List<LeaveConversionDetail>();
        }
    }
}