namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Authorize]
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