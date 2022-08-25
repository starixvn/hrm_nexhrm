namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LookupData;
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
            var criteria = new GetAllLookupDataCriteria { LookupCode = "JT" };
            IEnumerable<JobTitle> jobTitles = await baseService.GetDataAsync<JobTitle>(criteria);
            return jobTitles;
        }

        public async Task<IEnumerable<JobTitle>> GetJobTitle(GetJobTitleCriteria criteria)
        {
            IEnumerable<JobTitle> jobTitles = await baseService.GetDataAsync<JobTitle>(criteria);
            return jobTitles;
        }
    }
}