namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.LookupData;
    using Models;

    public interface IJobTitleService
    {
        IList<JobTitle> GetAll();

        IList<JobTitle> GetJobTitle(GetJobTitleCriteria criteria);
    }
}
