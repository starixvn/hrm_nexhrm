namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;

    public interface ISessionService
    {
        UserSession CurrentEmployee { get; set; }
        EmployeeSetting CurrentUserSetting { get; set; }
        void Clear();
    }
}
