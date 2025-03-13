namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Promotion;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PromotionService : IPromotionService
    {
        private readonly IBaseService baseService;

        public PromotionService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<IEnumerable<Promotion>> GetAll(GetPromotionCriteria criteria)
        {
            var promotions = await baseService.GetDataAsync<Promotion>(criteria);
            return promotions.ToList();
        }

        public async Task<Promotion> GetById(GetPromotionByIdCriteria criteria)
        {
            var promotion = await baseService.GetDataEntityAsync<Promotion>(criteria);
            return promotion;
        }

        public async Task<IList<JobGrade>> GetJobGrade()
        {
            var jobGrade = await baseService.GetDataAsync<JobGrade>(new GetJobGradeCriteria());
            return jobGrade.ToList();
        }

        public async Task<IList<ManagementGrade>> GetManagementGrade()
        {
            var managementGrades = await baseService.GetDataAsync<ManagementGrade>(new GetManagementGradeCriteria());
            return managementGrades.ToList();
        }
    }
}
