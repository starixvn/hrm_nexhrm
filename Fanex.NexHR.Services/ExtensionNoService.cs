namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.ExtensionNo;
    using Interfaces;
    using Models;

    public class ExtensionNoService : BaseService, IExtensionNoService
    {
        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria)
        {
            List<ExtensionNoModel> result = GetData<ExtensionNoModel>(criteria).ToList();
            return result;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria)
        {
            List<ExtensionNoModel> result = GetData<ExtensionNoModel>(criteria).ToList();
            return result;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria)
        {
            ExtensionNoModel result = GetDataEntity<ExtensionNoModel>(criteria);
            return result;
        }
    }
}
