namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class AuthService : IAuthService
    {
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public AuthService(IEmployeeService employeeService, IBaseService baseService)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
        }

        public async Task<Employee> GetEmployeeByDomain(string domainUserName)
        {
            var getEmployeeCriteria = new GetEmployeeByDomainNameCriteria { DomainUserName = domainUserName };
            var employee = await employeeService.GetEmployeeByDomainName(getEmployeeCriteria);
            return employee;
        }

        public async Task<EmployeeSetting> GetEmployeeSetting(int employeeId)
        {
            var employeeSettingCriteria = new GetCurrentUserSettingCriteria
            {
                EmployeeId = employeeId
            };
            var employeeSetting = await baseService.GetDataEntityAsync<EmployeeSetting>(employeeSettingCriteria);
            return employeeSetting;
        }
    }
}