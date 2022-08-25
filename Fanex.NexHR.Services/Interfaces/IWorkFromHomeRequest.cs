namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;

    public interface IWorkFromHomeRequest
    {
        Task<IEnumerable<WorkFromHomeRequest>> GetAll(GetWorkFromHomeRequestCriteria criteria);

        Task<WorkFromHomeRequest> GetById(int id);
    }
}
