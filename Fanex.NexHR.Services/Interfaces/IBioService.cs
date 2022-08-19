namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Bio;
    using Models.Bio;

    public interface IBioService
    {
        Task<IList<FingerprintAndCardSummary>> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria);

        Task<IEnumerable<FingerprintAndCardSummary>> GetWarningFingerprintAndCardSummaryAsync(GetFingerprintAndCardSummaryCriteria criteria);

        Task<IList<BioTimeChange>> GetBioTimeChanges(GetBioTimeChangeCriteria criteria);

        Task<IEnumerable<BioTimeChange>> GetBioTimeChangesAsync(GetBioTimeChangeCriteria criteria);
    }
}