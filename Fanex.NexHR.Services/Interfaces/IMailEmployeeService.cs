namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;

    public interface IMailEmployeeService
    {
        void SendMailUpdateSuccess(Employee employee);
    }
}
