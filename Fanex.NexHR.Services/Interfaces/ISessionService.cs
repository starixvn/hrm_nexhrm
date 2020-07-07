namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;

    public interface ISessionService
    {
        Employee CurrentEmployee { get; set; }
        EmployeeSetting CurrentUserSetting { get; set; }
        void Clear();
    }
}
