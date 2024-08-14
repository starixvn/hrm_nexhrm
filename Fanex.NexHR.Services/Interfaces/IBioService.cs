namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Bio;
    using Models.Bio;

    public interface IBioService
    {
        Task<IList<EmployeeFingerprintAndCardSummary>> GetFingerprintAndCardSummary(GetFingerprintAndCardSummaryCriteria criteria);
    }
}