namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enums;
    using Criteria.Bio;
    using Interfaces;
    using Models.Bio;
    using Repositories;
    using Repositories.Interfaces;

    public class BioService : BaseService, IBioService
    {
        private readonly IBioRepository _bioRepository;

        public BioService()
            : this(null)
        {
        }

        public BioService(IBioRepository bioRepository)
        {
            _bioRepository = bioRepository ?? new BioRepository();
        }

        public IList<FingerprintAndCardSummary> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria)
        {
            var fingerprintAndCardSummary = new List<FingerprintAndCardSummary>();

            if (criteria != null && criteria.IsValid())
            {
                fingerprintAndCardSummary = _bioRepository.GetFingerprintAndCardSummary(criteria).ToList();

                if (criteria.Type == FingerprintScannerType.Card)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => !string.IsNullOrEmpty(x.CardNo)).ToList();
                }

                if (criteria.Type == FingerprintScannerType.Fingerprint)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => x.TotalFingerprint > 0).ToList();
                }
            }

            return fingerprintAndCardSummary ?? new List<FingerprintAndCardSummary>();
        }

        public IList<BioTimeChange> GetBioTimeChanges(GetBioTimeChangeCriteria criteria)
        {
            IList<BioTimeChange> bioTimeChanges = null;

            if (criteria != null && criteria.IsValid())
            {
                bioTimeChanges = _bioRepository
                    .GetAllBioTimeChanges(criteria)
                    .OrderByDescending(x => x.TimeChange)
                    .ToList();
            }

            return bioTimeChanges ?? new List<BioTimeChange>();
        }
    }
}