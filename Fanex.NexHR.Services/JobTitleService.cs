namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

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
