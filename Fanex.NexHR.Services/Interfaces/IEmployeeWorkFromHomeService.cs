namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;

    public interface IEmployeeWorkFromHomeService
    {
        Task<IEnumerable<EmployeeWorkFromHome>> GetAll(GetEmployeeWorkFromHomeCriteria criteria);

        Task<EmployeeWorkFromHome> GetById(int id);
    }
}
