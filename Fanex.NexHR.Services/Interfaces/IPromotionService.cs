namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Promotion;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAll(GetPromotionCriteria criteria);

        Task<Promotion> GetById(GetPromotionByIdCriteria criteria);

        Task<IList<JobGrade>> GetJobGrade();
    }
}