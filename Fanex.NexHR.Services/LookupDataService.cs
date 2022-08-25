namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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

        public async Task<IEnumerable<LookupData>> GetAll(GetAllLookupDataCriteria criteria)
        {
            IEnumerable<LookupData> lookupDatas = await baseService.GetDataAsync<LookupData>(criteria);
            return lookupDatas;
        }
    }
}