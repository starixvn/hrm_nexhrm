namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Department;
    using Models;

    public interface IDepartmentService
    {
        IList<Department> GetAll();

        Department GetDepartmentById(GetDepartmentByIdCriteria criteria);
    }
}
