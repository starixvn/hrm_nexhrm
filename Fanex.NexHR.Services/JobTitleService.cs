namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.LookupData;
    using Interfaces;
    using Models;

    public class JobTitleService : BaseService, IJobTitleService
    {
        public IList<JobTitle> GetAll()
        {
            var criteria = new GetJobTitleCriteria();
            IList<JobTitle> jobTitles = GetData<JobTitle>(criteria);
            return jobTitles;
        }

        public IList<JobTitle> GetJobTitle(GetJobTitleCriteria criteria)
        {
            IList<JobTitle> jobTitles = GetData<JobTitle>(criteria);
            return jobTitles;
        }
    }
}
