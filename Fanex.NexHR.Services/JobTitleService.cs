namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class JobTitleService : BaseService, IJobTitleService
    {
        private readonly ILookupDataRepository _jobTitleRepository;

        public JobTitleService(ILookupDataRepository jobTitleRepository = null)
        {
            _jobTitleRepository = jobTitleRepository ?? new LookupDataRepository();
        }

        public IList<JobTitle> GetAll()
        {
            var criteria = new GetJobTitleCriteria();
            IList<JobTitle> jobTitles = null;
            if (criteria.IsValid())
            {
                jobTitles = GetData<JobTitle>(criteria).ToList();
            }

            return jobTitles ?? new List<JobTitle>();
        }

        public IList<JobTitle> GetJobTitle(GetJobTitleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<JobTitle> jobTitles = null;
            if (criteria.IsValid())
            {
                jobTitles = GetData<JobTitle>(criteria).ToList();
            }

            return jobTitles ?? new List<JobTitle>();
        }
    }
}
