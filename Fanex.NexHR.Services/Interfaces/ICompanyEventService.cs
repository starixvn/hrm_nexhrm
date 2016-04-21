namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.CompanyEvent;
    using Fanex.NexHR.Models.CompanyEvent;

    public interface ICompanyEventService
    {
        IList<CompanyEvent> GetAll(GetAllEventCriteria criteria);

        CompanyEvent GetById(GetEventByIdCriteria criteria);
    }
}
