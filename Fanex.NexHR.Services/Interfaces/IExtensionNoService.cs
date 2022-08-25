namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.ExtensionNo;
    using Models;

    public interface IExtensionNoService
    {
        Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNo(GetAllExtensionNoCriteria criteria);

        Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfo(GetAllExtensionGroupInfoCriteria criteria);

        Task<ExtensionNoModel> GetEmployeeExtensionNoById(GetEmployeeExtensionNoByIdCriteria criteria);
    }
}
