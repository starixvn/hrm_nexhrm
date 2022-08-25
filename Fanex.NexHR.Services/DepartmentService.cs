namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Department;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public async Task<IList<Department>> GetAll()
        {
            var criteria = new GetAllDepartmentCriteria();
            IList<Department> departments = null;

            if (criteria.IsValid())
            {
                departments = await departmentRepository.GetAll(criteria);
            }

            return departments ?? new List<Department>();
        }

        public async Task<Department> GetDepartmentById(GetDepartmentByIdCriteria criteria)
        {
            Department department = null;
            if (criteria != null && criteria.IsValid())
            {
                department = await departmentRepository.GetDepartmentById(criteria);
            }

            return department;
        }
    }
}