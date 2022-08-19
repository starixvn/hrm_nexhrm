namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
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

        public IList<CompanyEvent> GetAll(GetAllEventCriteria criteria)
        {
            return baseService.GetData<CompanyEvent>(criteria);
        }

        public CompanyEvent GetById(GetEventByIdCriteria criteria)
        {
            return baseService.GetDataEntity<CompanyEvent>(criteria);
        }
    }
}