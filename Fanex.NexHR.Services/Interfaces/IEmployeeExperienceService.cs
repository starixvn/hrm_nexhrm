namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.EmployeeExperience;
    using Fanex.NexHR.Models.EmployeeExperience;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmployeeExperienceService
    {
        Task<IEnumerable<EmployeeExperience>> GetAll(GetEmployeeExperienceCriteria criteria);
    }
}
