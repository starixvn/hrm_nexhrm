namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [HrmAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class BaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ISessionService sessionService;

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public UserSession CurrentUser
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