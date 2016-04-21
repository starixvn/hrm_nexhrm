namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.CompanyEvent;
    using Fanex.NexHR.Models.CompanyEvent;
    using Fanex.NexHR.Services.Interfaces;

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
