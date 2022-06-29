namespace Fanex.NexHR.Services
{
    using Criteria.Department;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public IList<Department> GetAll()
        {
            var criteria = new GetAllDepartmentCriteria();
            IList<Department> departments = null;

            if (criteria.IsValid())
            {
                departments = departmentRepository.GetAll(criteria);
            }

            return departments ?? new List<Department>();
        }

        public Department GetDepartmentById(GetDepartmentByIdCriteria criteria)
        {
            Department department = null;
            if (criteria != null && criteria.IsValid())
            {
                department = departmentRepository.GetDepartmentById(criteria);
            }

            return department;
        }
    }
}
