namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.LookupData;
    using Models;

    public interface ILookupDataService
    {
        IList<LookupData> GetAll(GetAllLookupDataCriteria criteria);
    }
}