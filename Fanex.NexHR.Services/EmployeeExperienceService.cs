namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.EmployeeExperience;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EmployeeExperienceService : IEmployeeExperienceService
    {
        private readonly IBaseService baseService;

        public EmployeeExperienceService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<IEnumerable<EmployeeExperience>> GetAll(GetEmployeeExperienceCriteria criteria)
        {
            var employeeExperiences = await baseService.GetDataAsync<EmployeeExperience>(criteria);
            return employeeExperiences.ToList();
        }

        public async Task<EmployeeExperience> GetById(GetEmployeeExperienceByIdCriteria criteria)
        {
            var employeeExperiences = await baseService.GetDataAsync<EmployeeExperience>(criteria);
            return employeeExperiences.First();
        }

        public async Task<IList<EmployeeExperience>> GetByEmpId(GetEmployeeExperienceByEmpIdCriteria criteria)
        {
            var employeeExperiences = await baseService.GetDataAsync<EmployeeExperience>(criteria);
            return employeeExperiences.ToList();
        }
    }
}
