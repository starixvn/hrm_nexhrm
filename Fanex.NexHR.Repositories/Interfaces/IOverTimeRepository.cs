namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Core.Enums;
    using Models;
    using ViewModels;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.OverTime;

    public interface IOverTimeRepository
    {
        IEnumerable<OverTime> GetSummaryReport(GetSummaryCriteria criteria);

        OverTime Get(GetByIdCriteria criteria);                
    }
}
