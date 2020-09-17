namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IYearlyNexRequestRepository
    {
        IList<YearlyNexRequest> GetYearlyNexRequest(ICriteria criteria);
    }
}
