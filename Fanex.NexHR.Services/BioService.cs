namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.Bio;
    using Interfaces;
    using Models.Bio;
    using Repositories;
    using Repositories.Interfaces;

    public class BioService : BaseService, IBioService
    {
        private readonly IReportRepository _reportRepository;

        public BioService()
            : this(null)
        {
        }

        public BioService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        public IList<FingerprintAndCardSummary> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria)
        {
            IList<FingerprintAndCardSummary> fingerprintAndCardSummary = null;

            if (criteria != null && criteria.IsValid())
            {
                fingerprintAndCardSummary = _reportRepository.GetFingerprintAndCardSummary(criteria);
            }

            return fingerprintAndCardSummary ?? new List<FingerprintAndCardSummary>();
        }
    }
}