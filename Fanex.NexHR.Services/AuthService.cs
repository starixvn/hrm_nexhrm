namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class AuthService : IAuthService
    {
        public Employee GetEmployeeByDomain(string domainUserName)
        {
            var getEmployeeCriteria = new GetEmployeeByDomainNameCriteria { DomainUserName = domainUserName };
            var employee = new EmployeeService().GetEmployeeByDomainName(getEmployeeCriteria);
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
