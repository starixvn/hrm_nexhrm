namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.CompanyEvent;
    using Interfaces;
    using Models.CompanyEvent;

    public class CompanyEventService : ICompanyEventService
    {
        private readonly IBaseService baseService;

        public CompanyEventService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<CompanyEvent>> GetAll(GetAllEventCriteria criteria)
        {
            return await baseService.GetDataAsync<CompanyEvent>(criteria);
        }

        public async Task<CompanyEvent> GetById(GetEventByIdCriteria criteria)
        {
            return await baseService.GetDataEntityAsync<CompanyEvent>(criteria);
        }
    }
}