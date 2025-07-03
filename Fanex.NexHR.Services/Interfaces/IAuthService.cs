namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IAuthService
    {
        Task<UserSession> GetEmployeeByDomain(string domainUserName);

        Task<EmployeeSetting> GetEmployeeSetting(int employeeId);

        Task<CommandResult> ChangePassword(int id, string password);

        Task<LoginEmployee> GetLoginEmployee(string domainUsername);

        Task AuthorizeGoSystemAsync();
    }
}