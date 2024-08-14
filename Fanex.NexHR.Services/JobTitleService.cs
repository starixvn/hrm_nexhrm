namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.JobTitle;
    using Interfaces;
    using Models;

    public class JobTitleService : IJobTitleService
    {
        private readonly IBaseService baseService;

        public JobTitleService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<JobTitle>> GetAll()
        {
            var criteria = new GetJobTitleCriteria();
            var jobTitles = await baseService.GetDataAsync<JobTitle>(criteria);
            return jobTitles;
        }

        public async Task<IEnumerable<JobTitle>> GetJobTitle(GetJobTitleCriteria criteria)
        {
            var jobTitles = await baseService.GetDataAsync<JobTitle>(criteria);
            return jobTitles;
        }
    }
}