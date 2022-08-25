namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.CompanyEvent;
    using Models.CompanyEvent;

    public interface ICompanyEventService
    {
        Task<IEnumerable<CompanyEvent>> GetAll(GetAllEventCriteria criteria);

        Task<CompanyEvent> GetById(GetEventByIdCriteria criteria);
    }
}
