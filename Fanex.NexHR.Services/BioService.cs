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

                if (criteria.Type == Core.Enums.FingerprintScannerType.Card)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => x.Card > 0).ToList();
                }

                if (criteria.Type == Core.Enums.FingerprintScannerType.Fingerprint)
                {
                    fingerprintAndCardSummary = fingerprintAndCardSummary.Where(x => x.Fingerprint > 0).ToList();
                }
            }

            return fingerprintAndCardSummary ?? new List<FingerprintAndCardSummary>();
        }

        public IList<Fingerprint> GetFingerprintDetailByEmployeeId(GetFingerprintDetailByEmployeeIdCriteria criteria)
        {
            var fingers = new List<Fingerprint>();
            if (criteria != null && criteria.IsValid())
            {
                fingers = _bioRepository.GetFingerprintDetailByEmpId(criteria).ToList();
            }

            return fingers;
        }

        public IList<Card> GetCardDetailByEmployeeId(GetCardDetailByEmployeeIdCriteria criteria)
        {
            IList<Card> fingers = null;
            if (criteria != null && criteria.IsValid())
            {
                fingers = _bioRepository.GetCardDetailByEmpId(criteria).ToList();
            }

            return fingers;
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