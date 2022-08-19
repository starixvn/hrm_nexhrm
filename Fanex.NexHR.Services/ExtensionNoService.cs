namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
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

        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria)
        {
            List<ExtensionNoModel> result = baseService.GetData<ExtensionNoModel>(criteria).ToList();
            return result;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria)
        {
            List<ExtensionNoModel> result = baseService.GetData<ExtensionNoModel>(criteria).ToList();
            return result;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria)
        {
            ExtensionNoModel result = baseService.GetDataEntity<ExtensionNoModel>(criteria);
            return result;
        }
    }
}