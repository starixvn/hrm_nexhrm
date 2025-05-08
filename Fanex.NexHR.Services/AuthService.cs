namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.Auth;
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

        public async Task<UserSession> GetEmployeeByDomain(string domainUserName)
        {
            var getUserCriteria = new GetEmployeeByDomainNameCriteria { DomainUserName = domainUserName };
            var userSession = await employeeService.GetEmployeeByDomainName(getUserCriteria);
            return userSession;
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

        public async Task<bool> IsLoginSuccess(string domainUsername, string password)
        {
            var criteria = new LoginCriteria
            {
                DomainUsername = domainUsername,
                Password = password
            };
            var isLoginSuccess = await baseService.GetDataEntityAsync<bool>(criteria);
            return isLoginSuccess;
        }

        public async Task<CommandResult> ChangePassword(int id, string password)
        {
            var criteria = new ChangePasswordCommand
            {
                Id = id,
                Password = password
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            return commandResult;
        }
    }
}