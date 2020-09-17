namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.YearlyNexRequest;
    using Fanex.NexHR.Models;

    public interface IYearlyNexRequestService
    {
        IList<YearlyNexRequest> GetYearlyNexRequest(GetYearlyNexRequestCriteria criteria);
    }
}
