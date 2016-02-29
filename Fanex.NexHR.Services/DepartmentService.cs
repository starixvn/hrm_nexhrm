namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private IDepartmentRepository departmentRepository;

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
