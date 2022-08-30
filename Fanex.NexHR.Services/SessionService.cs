namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Http;

    public class SessionService : ISessionService
    {
        private readonly ISession session;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        public Employee CurrentEmployee
        {
            get => session.Get<Employee>("CurrentUser");
            set => session.Set("CurrentUser", value);
        }

        public EmployeeSetting CurrentUserSetting
        {
            get => session.Get<EmployeeSetting>("CurrentUserSetting");
            set => session.Set("CurrentUserSetting", value);
        }

        public void Clear()
        {
            session.Clear();
        }
    }
}