namespace Fanex.NexHR.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.Auth;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using GOServices = GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Models.Auth;

    public class AuthService : IAuthService
    {
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly IUserControlSession userControlSession;
        private readonly GOServices.IAuthService goService;
        private readonly UserControlClientOptions options;
        private readonly GOServices.ICryptoService cryptoService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthService(IEmployeeService employeeService, 
            IBaseService baseService,
            IUserControlSession userControlSession,
            GOServices.IAuthService goService,
            IOptions<UserControlClientOptions> config,
            GOServices.ICryptoService cryptoService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
            this.userControlSession = userControlSession;
            this.goService = goService;
            options = config.Value;
            this.cryptoService = cryptoService;
            this.httpContextAccessor = httpContextAccessor;
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

        public async Task<LoginEmployee> GetLoginEmployee(string domainUsername)
        {
            var criteria = new GetLoginEmployeeCriteria
            {
                DomainUsername = domainUsername,
            };
            var loginEmployee = await baseService.GetDataEntityAsync<LoginEmployee>(criteria);
            return loginEmployee;
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

        public async Task AuthorizeGoSystemAsync()
        {
            var context = httpContextAccessor.HttpContext;
            var token = cryptoService.GenerateRijndaelKeyIV();
            var keyIv = token.Split(':');
            var rijndaelKey = keyIv[0];
            var rijndaelIv = keyIv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt(options.Username, rijndaelKey, rijndaelIv),
                Password = cryptoService.Encrypt(options.Password, rijndaelKey, rijndaelIv),
                Token = token,
                IP = GetClientIp(context),
                Platform = context.Request.Headers["User-Agent"].ToString(),
                BrowserName = context.Request.Headers["User-Agent"].ToString(),
                NetSessionId = context.Session.Id
            };
            var loginResult = await goService.LoginAsync(loginParam);
            userControlSession.UserId = loginResult.UserId;
            userControlSession.JwtToken = loginResult.JwtToken;
        }

        private static string GetClientIp(HttpContext httpContext)
        {
            var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            else
            {
                var forwardedIps = clientIp.Split(',', StringSplitOptions.RemoveEmptyEntries);

                clientIp = forwardedIps[0];
            }

            return clientIp;
        }
    }
}