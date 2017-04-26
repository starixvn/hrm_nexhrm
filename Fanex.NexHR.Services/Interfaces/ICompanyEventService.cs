namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.CompanyEvent;
    using Models.CompanyEvent;

    public interface ICompanyEventService
    {
        IList<CompanyEvent> GetAll(GetAllEventCriteria criteria);

        CompanyEvent GetById(GetEventByIdCriteria criteria);
    }
}
