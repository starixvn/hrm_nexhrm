namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class WorkFromHomeRequestService : IWorkFromHomeRequest
    {
        private readonly IBaseService baseService;

        public WorkFromHomeRequestService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<WorkFromHomeRequest>> GetAll(GetWorkFromHomeRequestCriteria criteria)
        {
            var wfhRequest = await baseService.GetDataAsync<WorkFromHomeRequest>(criteria);
            return wfhRequest.ToList();
        }

        public async Task<WorkFromHomeRequest> GetById(int id)
        {
            var criteria = new GetWorkFromHomeRequestByIdCriteria { Id = id };
            return await baseService.GetDataEntityAsync<WorkFromHomeRequest>(criteria);
        }
    }
}