namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Bio;
    using Models.Bio;

    public interface IBioService
    {
        IList<FingerprintAndCardSummary> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria);
    }
}