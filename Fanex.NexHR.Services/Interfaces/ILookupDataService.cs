namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;

    public interface ILookupDataService
    {
        IList<LookupData> GetAll(GetAllLookupDataCriteria criteria);
    }
}