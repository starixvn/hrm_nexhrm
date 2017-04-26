namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.LookupData;
    using Interfaces;
    using Models;

    public class LookupDataService : BaseService, ILookupDataService
    {
        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            IList<LookupData> lookupDatas = GetData<LookupData>(criteria);
            return lookupDatas;
        }
    }
}