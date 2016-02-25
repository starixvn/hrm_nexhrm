namespace Fanex.NexHR.Services
{
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
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                List<ExtensionNoModel> result = GetData<ExtensionNoModel>(criteria).ToList();
                return result;
            }

            return null;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                List<ExtensionNoModel> result = GetData<ExtensionNoModel>(criteria).ToList();
                return result;
            }

            return null;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                ExtensionNoModel result = GetDataEntity<ExtensionNoModel>(criteria);
                return result;
            }

            return null;
        }
    }
}
