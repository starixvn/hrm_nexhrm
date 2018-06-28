namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.Bio;

    public interface IBioRepository
    {
        IEnumerable<BioTimeChange> GetAllBioTimeChanges(ICriteria criteria);

        IEnumerable<FingerprintAndCardSummary> GetFingerprintAndCardSummary(ICriteria criteria);

        IEnumerable<Fingerprint> GetFingerprintDetailByEmpId(ICriteria criteria);

        IEnumerable<Card> GetCardDetailByEmpId(ICriteria criteria);
    }
}