namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.ExtensionNo;
    using Models;

    public interface IExtensionNoService
    {
        IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria);
    }
}
