namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Bio;
    using Interfaces;
    using Models.Bio;
    using Repositories.Interfaces;

    public class BioService : IBioService
    {
        private readonly IBioRepository bioRepository;

        public BioService(IBioRepository bioRepository)
        {
            this.bioRepository = bioRepository;
        }

        public async Task<IList<FingerprintAndCardSummary>> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria)
        {
            var fingerprintAndCardSummary = new List<FingerprintAndCardSummary>();

            if (criteria != null && criteria.IsValid())
            {
                var fingerprintAndCardSummaryAsync = await bioRepository.GetFingerprintAndCardSummary(criteria);
                fingerprintAndCardSummary = fingerprintAndCardSummaryAsync.ToList();

                if (criteria.Type == FingerprintScannerType.Card)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => !string.IsNullOrEmpty(x.CardNo)).ToList();
                }

                if (criteria.Type == FingerprintScannerType.Fingerprint)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => x.TotalFingerprint > 0).ToList();
                }

                if (criteria.Status == FingerprintScannerStatus.Pending)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => x.IsWarning).ToList();
                }

                if (criteria.Status == FingerprintScannerStatus.Approve)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => !x.IsWarning).ToList();
                }
            }

            return fingerprintAndCardSummary ?? new List<FingerprintAndCardSummary>();
        }

        public async Task<IEnumerable<FingerprintAndCardSummary>> GetWarningFingerprintAndCardSummaryAsync(GetFingerprintAndCardSummaryCriteria criteria)
        {
            IEnumerable<FingerprintAndCardSummary> fingerprintAndCardSummary = new List<FingerprintAndCardSummary>();

            if (criteria != null && criteria.IsValid())
            {
                fingerprintAndCardSummary = await bioRepository.GetFingerprintAndCardSummaryAsync(criteria).ConfigureAwait(false);
            }

            return fingerprintAndCardSummary.Where(x => x.IsWarning);
        }

        public async Task<IList<BioTimeChange>> GetBioTimeChanges(GetBioTimeChangeCriteria criteria)
        {
            IList<BioTimeChange> bioTimeChangesOrder = null;

            if (criteria != null && criteria.IsValid())
            {
                var bioTimeChanges = await bioRepository.GetAllBioTimeChanges(criteria);
                bioTimeChangesOrder = bioTimeChanges
                    .OrderByDescending(x => x.TimeChange)
                    .ToList();
            }

            return bioTimeChangesOrder ?? new List<BioTimeChange>();
        }

        public async Task<IEnumerable<BioTimeChange>> GetBioTimeChangesAsync(GetBioTimeChangeCriteria criteria)
        {
            IEnumerable<BioTimeChange> bioTimeChanges = new List<BioTimeChange>();

            if (criteria != null && criteria.IsValid())
            {
                bioTimeChanges = await bioRepository.GetAllBioTimeChangesAsync(criteria).ConfigureAwait(false);
            }

            return bioTimeChanges;
        }
    }
}