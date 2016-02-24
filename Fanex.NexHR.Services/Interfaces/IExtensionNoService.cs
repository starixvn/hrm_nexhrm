namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;

    public interface IExtensionNoService
    {
        IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNoCriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria);
    }
}
