namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IJobTitleService
    {
        IList<JobTitle> GetJobTitle(GetJobTitleCriteria criteria);
    }
}
