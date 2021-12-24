namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class WorkFromHomeRequestService : IWorkFromHomeRequest
    {
        private readonly ICommandService commandService;
        private readonly BaseService baseService;

        public WorkFromHomeRequestService()
            : this(null, null)
        {
        }

        public WorkFromHomeRequestService(ICommandService commandService, BaseService baseService)
        {
            this.commandService = commandService ?? new CommandService();
            this.baseService = baseService ?? new BaseService();
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

        public CommandResult Insert(InsertWorkFromHomeRequestCriteria criteria)
        {
            return commandService.Insert(criteria);
        }

        public CommandResult Update(EditWorkFromHomeRequestCriteria criteria)
        {
            return commandService.Update(criteria);
        }

        public CommandResult Delete(DeleteWorkFromHomeRequestCriteria criteria)
        {
            return commandService.Delete(criteria);
        }
    }
}
