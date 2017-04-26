namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.CompanyEvent;
    using Interfaces;
    using Models.CompanyEvent;

    public class CompanyEventService : BaseService, ICompanyEventService
    {
        public IList<CompanyEvent> GetAll(GetAllEventCriteria criteria)
        {
            return GetData<CompanyEvent>(criteria);
        }

        public CompanyEvent GetById(GetEventByIdCriteria criteria)
        {
            return GetDataEntity<CompanyEvent>(criteria);
        }
    }
}
