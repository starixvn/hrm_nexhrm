namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LookupData;
    using Models;

    public interface IJobTitleService
    {
        Task<IEnumerable<JobTitle>> GetAll();

        Task<IEnumerable<JobTitle>> GetJobTitle(GetJobTitleCriteria criteria);
    }
}
