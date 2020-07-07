namespace Fanex.NexHR.Services
{
    using System.Web;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class SessionService : ISessionService
    {
        public Employee CurrentEmployee
        {
            get => (Employee)HttpContext.Current.Session?["CurrentUser"];
            set => HttpContext.Current.Session["CurrentUser"] = value;
        }

        public EmployeeSetting CurrentUserSetting {
            get => (EmployeeSetting)HttpContext.Current.Session?["CurrentUserSetting"];
            set => HttpContext.Current.Session["CurrentUserSetting"] = value;
        }

        public void Clear()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
        }
    }
}
