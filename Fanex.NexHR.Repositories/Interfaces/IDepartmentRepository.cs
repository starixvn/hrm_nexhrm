namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models;

    public interface IDepartmentRepository
    {
        IList<Department> GetAll(ICriteria criteria);

        Department GetDepartmentById(ICriteria criteria);
    }
}
