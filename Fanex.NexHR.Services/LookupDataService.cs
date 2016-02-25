namespace Fanex.NexHR.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class LookupDataService : BaseService, ILookupDataService
    {
        public LookupDataService(ICommandService commandService)
        {
            this.commandService = commandService ?? new CommandService();
            this.cacheManager = CacheService.CreateInstance();
        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            IList<LookupData> lookupDatas = GetData<LookupData>(criteria);
            return lookupDatas;
        }
    }
}