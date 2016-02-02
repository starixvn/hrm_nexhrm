namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Core.Enums;
    using Models;
    using ViewModels;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;

    public interface IOvertimeRepository
    {
        IEnumerable<Overtime> GetSummaryReport(GetSummaryCriteria criteria);

        Overtime Get(GetByIdCriteria criteria);                
    }
}
