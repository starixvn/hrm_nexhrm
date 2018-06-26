namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Bio;
    using Models.Bio;

    public interface IBioService
    {
        IList<FingerprintAndCardSummary> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria);

        IList<Fingerprint> GetFingerprintDetailByEmployeeId(GetFingerprintDetailByEmployeeIdCriteria criteria);

        IList<Card> GetCardDetailByEmployeeId(GetCardDetailByEmployeeIdCriteria criteria);
        
        IList<BioTimeChange> GetBioTimeChanges(GetBioTimeChangeCriteria criteria);

    }
}