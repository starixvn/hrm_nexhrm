namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class BaseController : Controller
    {
        private readonly ISessionService sessionService;

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public Employee CurrentUser
        {
            get
            {
                return sessionService.CurrentEmployee;
            }
        }

        public EmployeeSetting CurrentUserSetting
        {
            get
            {
                return sessionService.CurrentUserSetting;
            }
        }
    }
}