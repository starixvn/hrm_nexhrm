namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class ExtensionNoService : BaseService, IExtensionNoService
    {
        private readonly IExtensionNoRepository _repository;

        public ExtensionNoService()
        {
            this._repository = new ExtensionNoRepository();
        }

        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = this._repository.GetAllExtensionNo(criteria);
                return result;
            }

            return null;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = this._repository.GetAllExtensionGroupInfo(criteria);
                return result;
            }

            return null;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                var result = this._repository.GetEmployeeExtensionNoById(criteria);
                return result;
            }

            return null;
        }
    }
}
