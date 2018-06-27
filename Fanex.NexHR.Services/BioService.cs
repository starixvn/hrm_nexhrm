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
            IList<FingerprintAndCardSummary> fingerprintAndCardSummary = null;

            if (criteria != null && criteria.IsValid())
            {
                fingerprintAndCardSummary = _bioRepository.GetFingerprintAndCardSummary(criteria);
            }

            return fingerprintAndCardSummary ?? new List<FingerprintAndCardSummary>();
        }

        public IList<Fingerprint> GetFingerprintDetailByEmployeeId(GetFingerprintDetailByEmployeeIdCriteria criteria)
        {
            IList<Fingerprint> fingers = null;
            if (criteria != null && criteria.IsValid())
            {
                fingers = _bioRepository.GetFingerprintDetailByEmpId(criteria);
            }

            return fingers;
        }

        public IList<Card> GetCardDetailByEmployeeId(GetCardDetailByEmployeeIdCriteria criteria)
        {
            IList<Card> fingers = null;
            if (criteria != null && criteria.IsValid())
            {
                fingers = _bioRepository.GetCardDetailByEmpId(criteria);
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