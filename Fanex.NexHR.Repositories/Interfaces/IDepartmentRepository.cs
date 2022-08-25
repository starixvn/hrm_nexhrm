namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDepartmentRepository
    {
        Task<IList<Department>> GetAll(ICriteria criteria);

        Task<Department> GetDepartmentById(ICriteria criteria);
    }
}
