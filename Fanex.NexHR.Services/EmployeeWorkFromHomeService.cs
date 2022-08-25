namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class EmployeeWorkFromHomeService : IEmployeeWorkFromHomeService
    {
        private readonly IBaseService baseService;

        public EmployeeWorkFromHomeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<EmployeeWorkFromHome>> GetAll(GetEmployeeWorkFromHomeCriteria criteria)
        {
            var employees = await baseService.GetDataAsync<EmployeeWorkFromHome>(criteria);
            return employees.ToList();
        }

        public async Task<EmployeeWorkFromHome> GetById(int id)
        {
            var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
            return await baseService.GetDataEntityAsync<EmployeeWorkFromHome>(criteria);
        }
    }
}