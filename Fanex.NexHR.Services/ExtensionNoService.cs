namespace Fanex.NexHR.Services
{
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ExtensionNoService : BaseService, IExtensionNoService
    {
        public ExtensionNoService()
        {
        }

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
