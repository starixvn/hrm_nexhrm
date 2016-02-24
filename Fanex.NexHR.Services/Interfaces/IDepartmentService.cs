namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;

    public interface IDepartmentService
    {
        IList<Department> GetAll();

        Department GetDepartmentById(GetDepartmentByIdCriteria criteria);
    }
}
