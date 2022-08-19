namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.LookupData;
    using Interfaces;
    using Models;

    public class LookupDataService : ILookupDataService
    {
        private readonly IBaseService baseService;

        public LookupDataService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            IList<LookupData> lookupDatas = baseService.GetData<LookupData>(criteria);
            return lookupDatas;
        }
    }
}