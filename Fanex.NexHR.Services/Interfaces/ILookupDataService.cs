namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LookupData;
    using Models;

    public interface ILookupDataService
    {
        Task<IEnumerable<LookupData>> GetAll(GetAllLookupDataCriteria criteria);

        IEnumerable<SalaryGroup> GetSalaryGroups();
    }
}