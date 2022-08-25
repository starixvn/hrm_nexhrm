namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.ExtensionNo;
    using Interfaces;
    using Models;

    public class ExtensionNoService : IExtensionNoService
    {
        private readonly IBaseService baseService;

        public ExtensionNoService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNo(GetAllExtensionNoCriteria criteria)
        {
            IEnumerable<ExtensionNoModel> result = await baseService.GetDataAsync<ExtensionNoModel>(criteria);
            return result.ToList();
        }

        public async Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria)
        {
            IEnumerable<ExtensionNoModel> result = await baseService.GetDataAsync<ExtensionNoModel>(criteria);
            return result.ToList();
        }

        public async Task<ExtensionNoModel> GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria)
        {
            ExtensionNoModel result = await baseService.GetDataEntityAsync<ExtensionNoModel>(criteria);
            return result;
        }
    }
}