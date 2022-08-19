namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IAuthService
    {
        Employee GetEmployeeByDomain(string domainUserName);

        Task<EmployeeSetting> GetEmployeeSetting(int employeeId);
    }
}