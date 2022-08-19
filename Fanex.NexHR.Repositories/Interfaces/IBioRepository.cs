namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.Bio;

    public interface IBioRepository
    {
        Task<IEnumerable<BioTimeChange>> GetAllBioTimeChanges(ICriteria criteria);

        Task<IEnumerable<BioTimeChange>> GetAllBioTimeChangesAsync(ICriteria criteria);

        Task<IEnumerable<FingerprintAndCardSummary>> GetFingerprintAndCardSummary(ICriteria criteria);

        Task<IEnumerable<FingerprintAndCardSummary>> GetFingerprintAndCardSummaryAsync(ICriteria criteria);
    }
}