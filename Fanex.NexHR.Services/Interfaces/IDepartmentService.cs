namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Department;
    using Models;

    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAll();

        Task<Department> GetDepartmentById(GetDepartmentByIdCriteria criteria);
    }
}
