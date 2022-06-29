namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class WorkFromHomeRequestService : IWorkFromHomeRequest
    {
        private readonly BaseService baseService;

        public WorkFromHomeRequestService(BaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<WorkFromHomeRequest> GetAll(GetWorkFromHomeRequestCriteria criteria)
        {
            return baseService.GetData<WorkFromHomeRequest>(criteria).ToList();
        }

        public WorkFromHomeRequest GetById(int id)
        {
            var criteria = new GetWorkFromHomeRequestByIdCriteria { Id = id };
            return baseService.GetDataEntity<WorkFromHomeRequest>(criteria);
        }
    }
}
