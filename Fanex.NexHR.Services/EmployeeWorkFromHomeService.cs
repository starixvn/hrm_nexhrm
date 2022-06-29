namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class EmployeeWorkFromHomeService : IEmployeeWorkFromHomeService
    {
        private readonly BaseService baseService;

        public EmployeeWorkFromHomeService(BaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<EmployeeWorkFromHome> GetAll(GetEmployeeWorkFromHomeCriteria criteria)
        {
            return baseService.GetData<EmployeeWorkFromHome>(criteria).ToList();
        }

        public EmployeeWorkFromHome GetById(int id)
        {
            var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
            return baseService.GetDataEntity<EmployeeWorkFromHome>(criteria);
        }
    }
}
