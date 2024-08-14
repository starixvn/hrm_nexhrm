namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.JobTitle;
    using Models;

    public interface IJobTitleService
    {
        Task<IEnumerable<JobTitle>> GetAll();

        Task<IEnumerable<JobTitle>> GetJobTitle(GetJobTitleCriteria criteria);
    }
}
