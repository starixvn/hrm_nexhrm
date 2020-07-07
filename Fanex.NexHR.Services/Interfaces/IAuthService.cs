namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;

    public interface IAuthService
    {
        Employee GetEmployeeByDomain(string domainUserName);
        EmployeeSetting GetEmployeeSetting(int employeeId);
    }
}
