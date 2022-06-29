namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;

    public interface IWorkFromHomeRequest
    {
        IList<WorkFromHomeRequest> GetAll(GetWorkFromHomeRequestCriteria criteria);

        WorkFromHomeRequest GetById(int id);
    }
}
