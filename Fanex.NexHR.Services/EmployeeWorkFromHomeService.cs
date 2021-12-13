namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class EmployeeWorkFromHomeService : IEmployeeWorkFromHomeService
    {
        private readonly ICommandService commandService;
        private readonly BaseService baseService;

        public EmployeeWorkFromHomeService()
            : this(null, null)
        {
        }

        public EmployeeWorkFromHomeService(ICommandService commandService, BaseService baseService)
        {
            this.commandService = commandService ?? new CommandService();
            this.baseService = baseService ?? new BaseService();
        }

        public IList<EmployeeWorkFromHome> GetAll(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetEmployeeWorkFromHomeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            return baseService.GetData<EmployeeWorkFromHome>(criteria).ToList();
        }

        public EmployeeWorkFromHome GetById(int id)
        {
            var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
            return baseService.GetDataEntity<EmployeeWorkFromHome>(criteria);
        }

        public CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria)
        {
            return commandService.Insert(criteria);
        }

        public CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria)
        {
            return commandService.Update(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };

            return commandService.Delete(criteria);
        }
    }
}
