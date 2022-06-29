namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class AuthService : IAuthService
    {
        private readonly IEmployeeService employeeService;

        public AuthService(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        public Employee GetEmployeeByDomain(string domainUserName)
        {
            var getEmployeeCriteria = new GetEmployeeByDomainNameCriteria { DomainUserName = domainUserName };
            var employee = employeeService.GetEmployeeByDomainName(getEmployeeCriteria);
            return employee;
        }

        public EmployeeSetting GetEmployeeSetting(int employeeId)
        {
            var employeeSettingCriteria = new GetCurrentUserSettingCriteria { EmployeeId = employeeId };
            var baseService = new BaseService();
            var employeeSetting = baseService.GetDataEntity<EmployeeSetting>(employeeSettingCriteria);
            return employeeSetting;
        }
    }
}
