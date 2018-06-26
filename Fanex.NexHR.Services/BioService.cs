namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.Bio;
    using Interfaces;
    using Models.Bio;
    using Repositories;
    using Repositories.Interfaces;

    public class BioService : BaseService, IBioService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IBioRepository _bioRepository;

        public BioService()
            : this(null, null)
        {
        }

        public BioService(IReportRepository reportRepository, IBioRepository bioRepository)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
            _bioRepository = bioRepository ?? new BioRepository();
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

        public IList<BioTimeChange> GetBioTimeChanges(GetBioTimeChangeCriteria criteria)
        {
            IList<BioTimeChange> bioTimeChanges = null;

            if (criteria != null && criteria.IsValid())
            {
                bioTimeChanges = _bioRepository.GetAllBioTimeChanges(criteria).ToList();
            }

            return bioTimeChanges ?? new List<BioTimeChange>();
        }
    }
}