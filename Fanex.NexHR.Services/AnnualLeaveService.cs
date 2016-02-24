namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models.AnnualLeave;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class AnnualLeaveService : BaseService, IAnnualLeaveService
    {
        private readonly IAnnualLeaveRepository annualLeaveRepository;

        public AnnualLeaveService(IAnnualLeaveRepository annualLeaveRepository = null)
        {
            this.annualLeaveRepository = annualLeaveRepository ?? new AnnualLeaveRepository();
        }

        public IList<LeaveSummary> GetAnnualLeaveBalance(GetAnnualLeaveSummaryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LeaveSummary> leaveSummary = null;
            if (criteria.IsValid())
            {
                leaveSummary = annualLeaveRepository.GetAnnualLeaveBalance(criteria);
            }

            return leaveSummary ?? new List<LeaveSummary>();
        }

        public ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            ShowOpenAndCloseAnnualBalance showOpenAndCloseAnnualBalance = null;
            if (criteria.IsValid())
            {
                showOpenAndCloseAnnualBalance = GetDataEntity<ShowOpenAndCloseAnnualBalance>(criteria);
            }

            return showOpenAndCloseAnnualBalance;
        }

        public IList<AnnualLeaveTransaction> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<AnnualLeaveTransaction> annualLeaves = null;

            if (criteria.IsValid())
            {
                annualLeaves = annualLeaveRepository.GetAnnualLeaveTransactions(criteria);
            }

            return annualLeaves ?? new List<AnnualLeaveTransaction>();
        }

        public AnnualLeaveTransaction GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            AnnualLeaveTransaction annualLeave = null;
            if (criteria.IsValid())
            {
                annualLeave = annualLeaveRepository.GetAnnualLeaveTransactionById(criteria);
            }

            return annualLeave;
        }
    }
}