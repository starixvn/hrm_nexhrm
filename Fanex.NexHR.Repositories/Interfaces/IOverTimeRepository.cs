namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.NexHR.Criteria.Overtime;
    using Models;
    using System.Collections.Generic;

    public interface IOvertimeRepository
    {
        IEnumerable<Overtime> GetSummaryReport(GetSummaryCriteria criteria);

        Overtime Get(GetByIdCriteria criteria);                
    }
}
