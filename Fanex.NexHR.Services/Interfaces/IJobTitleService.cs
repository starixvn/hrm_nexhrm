namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;

    public interface IJobTitleService
    {
        IList<JobTitle> GetAll();

        IList<JobTitle> GetJobTitle(GetJobTitleCriteria criteria);
    }
}
