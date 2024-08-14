namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Bio;
    using Interfaces;
    using Models.Bio;

    public class BioService : IBioService
    {
        private readonly IBaseService baseService;

        public BioService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IList<EmployeeFingerprintAndCardSummary>> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria)
        {
            var fingerprintAndCardSummary = new List<EmployeeFingerprintAndCardSummary>();

            if (criteria != null && criteria.IsValid())
            {
                var fingerprintAndCardSummaryAsync = await baseService.GetDataAsync<EmployeeFingerprintAndCardSummary>(criteria);
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

            return fingerprintAndCardSummary;
        }
    }
}