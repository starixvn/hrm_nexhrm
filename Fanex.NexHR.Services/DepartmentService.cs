namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Department;
    using Interfaces;
    using Models;

    public class DepartmentService : IDepartmentService
    {
        private readonly IBaseService baseService;

        public DepartmentService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Department>> GetAll(bool? isSpecial = false)
        {
            var criteria = new GetAllDepartmentCriteria { IsDisabled = false, IsSpecial = isSpecial };
            var departments = await baseService.GetDataAsync<Department>(criteria);

            return departments;
        }

        public async Task<Department> GetDepartmentById(GetDepartmentByIdCriteria criteria)
        {
            var department = await baseService.GetDataEntityAsync<Department>(criteria);
            return department;
        }
    }
}