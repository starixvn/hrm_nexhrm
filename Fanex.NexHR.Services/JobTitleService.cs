namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
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

        public IList<JobTitle> GetAll()
        {
            var criteria = new GetAllLookupDataCriteria { LookupCode = "JT" };
            IList<JobTitle> jobTitles = baseService.GetData<JobTitle>(criteria);
            return jobTitles;
        }

        public IList<JobTitle> GetJobTitle(GetJobTitleCriteria criteria)
        {
            IList<JobTitle> jobTitles = baseService.GetData<JobTitle>(criteria);
            return jobTitles;
        }
    }
}