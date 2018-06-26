namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.Bio;

    public interface IBioRepository
    {
    	IEnumerable<BioTimeChange> GetAllBioTimeChanges(ICriteria criteria);
    	
        IList<FingerprintAndCardSummary> GetFingerprintAndCardSummary(ICriteria criteria);

        IList<Fingerprint> GetFingerprintDetailByEmpId(ICriteria criteria);

        IList<Card> GetCardDetailByEmpId(ICriteria criteria);
    }
}